using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Michitai.Multiplayer.Steam
{
    /// <summary>
    /// Exception thrown when a Steam operation fails (init error, ticket
    /// callback returning a non-OK EResult, malformed ticket data).
    /// </summary>
    public class SteamException : Exception
    {
        /// <summary>Creates a new SteamException with the given message.</summary>
        public SteamException(string message) : base(message) { }
    }

    /// <summary>
    /// A single-use Steam web auth ticket from ISteamUser::GetAuthTicketForWebApi.
    /// Pass <see cref="Hex"/> to Players.RegisterPlayerWithSteam as steamTicket,
    /// then Dispose to release the handle via CancelAuthTicket — always dispose,
    /// on success and failure, or ticket handles leak Steam-side quota.
    /// </summary>
    public sealed class SteamAuthTicket : IDisposable
    {
        /// <summary>Lowercase hex encoding of exactly m_cubTicket ticket bytes.</summary>
        public string Hex { get; private set; }

        /// <summary>HAuthTicket handle returned by GetAuthTicketForWebApi.</summary>
        public uint Handle { get; private set; }

        internal SteamAuthTicket(string hex, uint handle)
        {
            Hex = hex;
            Handle = handle;
        }

        /// <summary>Cancels the ticket handle. Safe to call more than once.</summary>
        public void Dispose()
        {
            SteamTicket.CancelAuthTicket(Handle);
            Handle = 0;
        }
    }

    /// <summary>
    /// Minimal Steam client shim: initializes the Steamworks API, pumps callbacks
    /// via manual dispatch, and mints single-use web auth tickets for
    /// Players.RegisterPlayerWithSteam. Auth-only by design — it is not a lobby,
    /// overlay or stats binding.
    ///
    /// Requirements: steam_api64.dll (x64) or steam_api.dll (x86) from the
    /// Steamworks SDK redistributable_bin next to the executable, a
    /// steam_appid.txt beside it when not launched through Steam, and the Steam
    /// client installed and logged in. In Unity, <see cref="SteamBehaviour"/>
    /// calls <see cref="RunFrame"/> and <see cref="Shutdown"/> automatically;
    /// .NET consumers must call <see cref="RunFrame"/> on their own loop/timer.
    /// </summary>
    public static class SteamTicket
    {
        /// <summary>k_iSteamUserCallbacks (100) + 68 — GetTicketForWebApiResponse_t.</summary>
        const int GetTicketForWebApiResponseCallback = 168;

        /// <summary>k_iSteamFriendsCallbacks (300) + 37 — GameRichPresenceJoinRequested_t.
        /// Fired when the local user accepts a rich-presence "join" invite
        /// (SteamFriends.OpenInviteDialog connect strings land here).</summary>
        const int RichPresenceJoinRequestedCallback = 337;

        const int EResultOK = 1;
        const int MaxTicketBytes = 8192;

        /// <summary>GameRichPresenceJoinRequested_t.m_rgchConnect is
        /// k_cchMaxRichPresenceValueLength chars after m_steamIDFriend.</summary>
        const int ConnectStringMaxBytes = 256;

        static readonly TimeSpan DefaultTimeout = TimeSpan.FromSeconds(10);

        sealed class PendingTicket
        {
            public TaskCompletionSource<SteamAuthTicket> Tcs;
            public DateTime DeadlineUtc;
            public CancellationTokenRegistration CancelReg;
        }

        static bool _initialized;
        static int _pipe;
        static IntPtr _user;
        static IntPtr _friends;
        static IntPtr _apps;
        static readonly Dictionary<uint, PendingTicket> _pending = new Dictionary<uint, PendingTicket>();

        /// <summary>True after a successful Initialize — Steam client is running and bound.</summary>
        public static bool Available { get { return _initialized; } }

        /// <summary>Local user's SteamID64, or 0 when Steam is unavailable.</summary>
        public static ulong SteamId { get; private set; }

        /// <summary>Local user's Steam persona name, or null when unavailable.</summary>
        public static string PersonaName { get; private set; }

        /// <summary>Steam client's per-game language pick (e.g. "english",
        /// "schinese") from the game properties language dropdown — null when
        /// unavailable. Set during Initialize.</summary>
        public static string GameLanguage { get; private set; }

        /// <summary>The bound ISteamFriends interface pointer — used by
        /// SteamFriends; IntPtr.Zero when unavailable.</summary>
        internal static IntPtr FriendsHandle { get { return _friends; } }

        /// <summary>ESteamAPIInitResult of the last SteamAPI_InitFlat call
        /// (0=OK, 1=FailedGeneric, 2=NoSteamClient, 3=VersionMismatch), or -1 when
        /// init never ran or the native library is missing.</summary>
        public static int InitResult { get; private set; }

        /// <summary>Human-readable reason the last Initialize failed, or null.</summary>
        public static string InitError { get; private set; }

        /// <summary>
        /// Raised on the callback-pump thread (Unity: the main thread via
        /// SteamBehaviour.Update) when the user accepts a rich-presence join
        /// invite. Parameters: inviter SteamID64, the connect string the
        /// inviter published (see SteamFriends.SetJoinConnectString).
        /// Games launched fresh by the invite get the same string as the
        /// "+connect" launch parameter instead — check Environment args too.
        /// </summary>
        public static event Action<ulong, string> RichPresenceJoinRequested;

        static SteamTicket()
        {
            InitResult = -1;
        }

        /// <summary>
        /// Returns true if the Steam client should relaunch the game through
        /// Steam (dev launches outside the client). Optional release-build guard.
        /// </summary>
        /// <param name="appId">The game's Steam AppID.</param>
        public static bool RestartAppIfNecessary(uint appId)
        {
            return SteamNative.Loaded && SteamNative.RestartAppIfNecessary(appId);
        }

        /// <summary>
        /// Loads the Steamworks library and initializes the API. Idempotent —
        /// returns true once initialized; retries when a previous attempt failed
        /// (e.g. the Steam client was still starting). False means no Steam:
        /// the game decides the offline/guest policy, not the SDK.
        /// </summary>
        public static bool Initialize()
        {
            if (_initialized) return true;
            if (!SteamNative.Load())
            {
                InitError = SteamNative.LoadError;
                return false;
            }

            int result;
            var errMsg = new byte[1024]; // SteamErrMsg = char[1024]
            try
            {
                result = SteamNative.InitFlat(errMsg);
            }
            catch (Exception ex)
            {
                InitError = ex.Message;
                return false;
            }

            InitResult = result;
            if (result != 0)
            {
                InitError = CStr(errMsg) ?? ("ESteamAPIInitResult " + result);
                return false;
            }

            SteamNative.ManualDispatchInit(); // mutually exclusive with SteamAPI_RunCallbacks
            _pipe = SteamNative.GetHSteamPipe();
            _user = SteamNative.SteamUser();
            _friends = SteamNative.SteamFriends();
            if (SteamNative.SteamApps != null && SteamNative.GetCurrentGameLanguage != null)
            {
                _apps = SteamNative.SteamApps();
                GameLanguage = PtrToUtf8(SteamNative.GetCurrentGameLanguage(_apps));
            }
            SteamId = SteamNative.GetSteamID(_user);
            PersonaName = PtrToUtf8(SteamNative.GetPersonaName(_friends));
            InitError = null;
            _initialized = true;
            return true;
        }

        /// <summary>
        /// Mints a fresh single-use web auth ticket. Await it, pass
        /// <see cref="SteamAuthTicket.Hex"/> to RegisterPlayerWithSteam together
        /// with the same identity string, then dispose the ticket. Never cache
        /// or retry a ticket — request a new one per login.
        /// </summary>
        /// <param name="identity">Must match the steam_identity the server passes to
        /// AuthenticateUserTicket — keep the default "michitai".</param>
        /// <param name="ct">Cancellation token; cancels the pending Steam handle too.</param>
        /// <returns>The ticket; throws SteamException/TimeoutException on failure.</returns>
        public static Task<SteamAuthTicket> GetAuthTicketAsync(string identity = "michitai", CancellationToken ct = default)
        {
            if (string.IsNullOrEmpty(identity)) identity = "michitai";
            if (!Initialize())
                return Task.FromException<SteamAuthTicket>(new InvalidOperationException(
                    "Steam is not available: " + (InitError ?? "initialization failed")));

            var identityBytes = Encoding.UTF8.GetBytes(identity + "\0");
            uint handle;
            try
            {
                handle = SteamNative.GetAuthTicketForWebApi(_user, identityBytes);
            }
            catch (Exception ex)
            {
                return Task.FromException<SteamAuthTicket>(ex);
            }
            if (handle == 0)
                return Task.FromException<SteamAuthTicket>(
                    new SteamException("GetAuthTicketForWebApi returned an invalid handle."));

            var tcs = new TaskCompletionSource<SteamAuthTicket>(TaskCreationOptions.RunContinuationsAsynchronously);
            var pending = new PendingTicket
            {
                Tcs = tcs,
                DeadlineUtc = DateTime.UtcNow + DefaultTimeout
            };
            lock (_pending) _pending[handle] = pending;
            if (ct.CanBeCanceled)
            {
                var capturedHandle = handle;
                pending.CancelReg = ct.Register(() =>
                    CancelPending(capturedHandle, () => tcs.TrySetCanceled(ct)));
            }
            return tcs.Task;
        }

        /// <summary>
        /// Pumps the Steam manual-dispatch queue — call once per frame on the
        /// same thread that called Initialize (Unity: automatic via
        /// SteamBehaviour). Do NOT also call SteamAPI_RunCallbacks; manual
        /// dispatch replaces it.
        /// </summary>
        public static void RunFrame()
        {
            if (!_initialized) return;
            SteamNative.ManualDispatchRunFrame(_pipe);
            ExpirePending();

            var msg = new SteamNative.CallbackMsg();
            while (SteamNative.ManualDispatchGetNextCallback(_pipe, ref msg))
            {
                try
                {
                    if (msg.Callback == GetTicketForWebApiResponseCallback && msg.ParamSize >= 12)
                        DispatchTicket(msg.Param);
                    else if (msg.Callback == RichPresenceJoinRequestedCallback && msg.ParamSize >= 9)
                        DispatchJoinRequested(msg.Param, msg.ParamSize);
                    // all other callback ids are irrelevant — ignore
                }
                finally
                {
                    SteamNative.ManualDispatchFreeLastCallback(_pipe);
                }
            }
        }

        /// <summary>
        /// Fails all pending ticket requests and shuts the Steam API down.
        /// Called automatically on application quit by SteamBehaviour.
        /// </summary>
        public static void Shutdown()
        {
            if (!_initialized) return;

            // Cancel every outstanding ticket handle while the pipe is still
            // valid — SteamAPI_Shutdown would drop them locally, but the
            // Steam-side handle quota only releases on explicit cancel.
            List<KeyValuePair<uint, PendingTicket>> rest;
            lock (_pending)
            {
                rest = new List<KeyValuePair<uint, PendingTicket>>(_pending);
                _pending.Clear();
            }
            foreach (var kv in rest)
            {
                kv.Value.CancelReg.Dispose();
                try { SteamNative.CancelAuthTicket(_user, kv.Key); } catch { }
                kv.Value.Tcs.TrySetCanceled();
            }

            _initialized = false; // SteamAuthTicket.Dispose must skip native calls now

            try { SteamNative.Shutdown(); } catch { }
            _user = IntPtr.Zero;
            _friends = IntPtr.Zero;
            _apps = IntPtr.Zero;
            _pipe = 0;
            SteamId = 0;
            PersonaName = null;
            GameLanguage = null;
        }

        internal static void CancelAuthTicket(uint handle)
        {
            if (!_initialized || handle == 0) return;
            try { SteamNative.CancelAuthTicket(_user, handle); } catch { }
        }

        // ---- internals -----------------------------------------------------

        static void DispatchTicket(IntPtr param)
        {
            // GetTicketForWebApiResponse_t: uint32 hAuthTicket, int32 eResult,
            // int32 cubTicket, then cubTicket bytes. Read by offset instead of
            // declaring m_rgubTicket — k_nCubTicketMaxLength changes between
            // SDK releases (2560 in 1.65), the three prefix fields do not.
            uint handle = (uint)Marshal.ReadInt32(param, 0);
            int result = Marshal.ReadInt32(param, 4);
            int len = Marshal.ReadInt32(param, 8);

            PendingTicket pending = TakePending(handle);
            if (pending == null) return; // late or duplicate callback for a canceled/timed-out request
            pending.CancelReg.Dispose();

            if (result != EResultOK)
            {
                pending.Tcs.TrySetException(new SteamException(
                    "GetAuthTicketForWebApi failed with EResult " + result + "."));
            }
            else if (len <= 0 || len > MaxTicketBytes)
            {
                pending.Tcs.TrySetException(new SteamException(
                    "GetAuthTicketForWebApi returned an invalid ticket size (" + len + ")."));
            }
            else
            {
                var bytes = new byte[len]; // exactly m_cubTicket — the buffer is padded
                Marshal.Copy(IntPtr.Add(param, 12), bytes, 0, len);
                pending.Tcs.TrySetResult(new SteamAuthTicket(ToHex(bytes), handle));
            }
        }

        static void DispatchJoinRequested(IntPtr param, int paramSize)
        {
            // GameRichPresenceJoinRequested_t: CSteamID m_steamIDFriend (8),
            // char m_rgchConnect[256]. Read the connect string bounded by the
            // reported param size — older SDKs may report a shorter array.
            ulong friendId = (ulong)Marshal.ReadInt64(param, 0);
            int maxLen = Math.Min(ConnectStringMaxBytes, paramSize - 8);
            var buf = new byte[maxLen];
            Marshal.Copy(IntPtr.Add(param, 8), buf, 0, maxLen);
            string connect = CStr(buf);
            RichPresenceJoinRequested?.Invoke(friendId, connect ?? "");
        }

        static void ExpirePending()
        {
            var now = DateTime.UtcNow;
            List<uint> expired = null;
            lock (_pending)
            {
                foreach (var kv in _pending)
                {
                    if (kv.Value.DeadlineUtc <= now)
                    {
                        if (expired == null) expired = new List<uint>();
                        expired.Add(kv.Key);
                    }
                }
            }
            if (expired == null) return;
            foreach (var handle in expired)
            {
                var p = TakePending(handle);
                if (p == null) continue;
                p.CancelReg.Dispose();
                CancelAuthTicket(handle);
                p.Tcs.TrySetException(new TimeoutException(
                    "Steam auth ticket request timed out — is the Steam client running?"));
            }
        }

        static void CancelPending(uint handle, Action complete)
        {
            var p = TakePending(handle);
            if (p == null) return;
            p.CancelReg.Dispose();
            CancelAuthTicket(handle);
            complete();
        }

        static PendingTicket TakePending(uint handle)
        {
            lock (_pending)
            {
                PendingTicket p;
                if (!_pending.TryGetValue(handle, out p)) return null;
                _pending.Remove(handle);
                return p;
            }
        }

        static string ToHex(byte[] data)
        {
            var sb = new StringBuilder(data.Length * 2);
            foreach (byte b in data) sb.Append(b.ToString("x2"));
            return sb.ToString();
        }

        static string CStr(byte[] buf)
        {
            int n = Array.IndexOf(buf, (byte)0);
            if (n < 0) n = buf.Length;
            return n == 0 ? null : Encoding.UTF8.GetString(buf, 0, n);
        }

        static string PtrToUtf8(IntPtr p)
        {
            if (p == IntPtr.Zero) return null;
            int n = 0;
            while (Marshal.ReadByte(p, n) != 0) n++;
            var buf = new byte[n];
            Marshal.Copy(p, buf, 0, n);
            return Encoding.UTF8.GetString(buf);
        }
    }
}
