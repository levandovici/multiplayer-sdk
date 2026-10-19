using System;
using System.Runtime.InteropServices;

namespace Michitai.Multiplayer.Steam
{
    /// <summary>
    /// Internal bindings to the Steamworks flat C API. The native library is loaded
    /// at runtime (LoadLibrary/dlopen) so the correct binary name can be chosen per
    /// platform — steam_api64.dll, steam_api.dll, libsteam_api.so, libsteam_api.dylib —
    /// and a missing Steam install reports a clean failure instead of a
    /// DllNotFoundException mid-call.
    /// Export names verified against Steamworks SDK 1.65 (steam_api_flat.h,
    /// steam_api.h, steam_api_internal.h). Requires SDK >= 1.57 for
    /// GetAuthTicketForWebApi.
    /// </summary>
    internal static class SteamNative
    {
        const int RtldNow = 2;

        static IntPtr _lib;
        static bool _loadAttempted;
        static string _loadError;

        /// <summary>True once the native library has been loaded and all exports bound.</summary>
        public static bool Loaded { get { return _lib != IntPtr.Zero; } }

        /// <summary>Why the last Load attempt failed, or null.</summary>
        public static string LoadError { get { return _loadError; } }

        [StructLayout(LayoutKind.Sequential)]
        internal struct CallbackMsg
        {
            public int SteamUser;    // HSteamUser m_hSteamUser
            public int Callback;     // int m_iCallback
            public IntPtr Param;     // uint8* m_pubParam
            public int ParamSize;    // int m_cubParam
        }

        // ---- platform loader imports ---------------------------------------

        [DllImport("kernel32", SetLastError = true, CharSet = CharSet.Ansi)]
        static extern IntPtr LoadLibrary(string lpFileName);

        [DllImport("kernel32", SetLastError = true, CharSet = CharSet.Ansi)]
        static extern IntPtr GetProcAddress(IntPtr hModule, string lpProcName);

        [DllImport("libdl", CharSet = CharSet.Ansi)]
        static extern IntPtr dlopen(string filename, int flags);

        [DllImport("libdl", CharSet = CharSet.Ansi)]
        static extern IntPtr dlsym(IntPtr handle, string symbol);

        static bool IsWindows
        {
            get { return RuntimeInformation.IsOSPlatform(OSPlatform.Windows); }
        }

        // ---- bound exports --------------------------------------------------

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate bool RestartAppIfNecessaryFn(uint ownAppId);
        internal static RestartAppIfNecessaryFn RestartAppIfNecessary;

        /// <summary>SteamAPI_InitFlat — the S_API export intended for dynamic loading.
        /// SteamAPI_Init itself is an inline C++ wrapper since SDK 1.59 and is NOT
        /// exported by the DLL. Returns ESteamAPIInitResult (0 = OK).</summary>
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate int InitFlatFn([Out] byte[] errMsg);
        internal static InitFlatFn InitFlat;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate void ShutdownFn();
        internal static ShutdownFn Shutdown;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate int GetHSteamPipeFn();
        internal static GetHSteamPipeFn GetHSteamPipe;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate void ManualDispatchInitFn();
        internal static ManualDispatchInitFn ManualDispatchInit;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate void ManualDispatchRunFrameFn(int pipe);
        internal static ManualDispatchRunFrameFn ManualDispatchRunFrame;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate bool ManualDispatchGetNextCallbackFn(int pipe, ref CallbackMsg msg);
        internal static ManualDispatchGetNextCallbackFn ManualDispatchGetNextCallback;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate void ManualDispatchFreeLastCallbackFn(int pipe);
        internal static ManualDispatchFreeLastCallbackFn ManualDispatchFreeLastCallback;

        /// <summary>SteamAPI_SteamUser_v023 — ISteamUser accessor (SteamUser023 in SDK 1.65).</summary>
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate IntPtr SteamUserFn();
        internal static SteamUserFn SteamUser;

        /// <summary>SteamAPI_SteamFriends_v018 — ISteamFriends accessor (SteamFriends018 in SDK 1.65).</summary>
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate IntPtr SteamFriendsFn();
        internal static SteamFriendsFn SteamFriends;

        /// <summary>SteamAPI_SteamApps_vNNN — ISteamApps accessor (version-suffixed; TryBind probes v009…v005).</summary>
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate IntPtr SteamAppsFn();
        internal static SteamAppsFn SteamApps;

        /// <summary>Returns a const char* (UTF-8) with the per-game language
        /// key ("english", "schinese", ...), set by the client's language
        /// dropdown for this game.</summary>
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate IntPtr GetCurrentGameLanguageFn(IntPtr self);
        internal static GetCurrentGameLanguageFn GetCurrentGameLanguage;

        /// <summary>identity is a NUL-terminated UTF-8 buffer (const char*).</summary>
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate uint GetAuthTicketForWebApiFn(IntPtr self, byte[] identity);
        internal static GetAuthTicketForWebApiFn GetAuthTicketForWebApi;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate ulong GetSteamIDFn(IntPtr self);
        internal static GetSteamIDFn GetSteamID;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate void CancelAuthTicketFn(IntPtr self, uint ticket);
        internal static CancelAuthTicketFn CancelAuthTicket;

        /// <summary>Returns a const char* (UTF-8) persona name.</summary>
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate IntPtr GetPersonaNameFn(IntPtr self);
        internal static GetPersonaNameFn GetPersonaName;

        // ---- ISteamFriends (SteamFriends018, SDK 1.65) ---------------------

        /// <summary>iFriendFlags is an EFriendFlags bitmask (0xFFFF = all,
        /// 0x04 = immediate friends).</summary>
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate int GetFriendCountFn(IntPtr self, int iFriendFlags);
        internal static GetFriendCountFn GetFriendCount;

        /// <summary>Returns the friend's SteamID64 (CSteamID, marshalled as uint64).</summary>
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate ulong GetFriendByIndexFn(IntPtr self, int iFriend, int iFriendFlags);
        internal static GetFriendByIndexFn GetFriendByIndex;

        /// <summary>Returns EFriendRelationship (3 = friend).</summary>
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate int GetFriendRelationshipFn(IntPtr self, ulong steamIDFriend);
        internal static GetFriendRelationshipFn GetFriendRelationship;

        /// <summary>Returns EPersonaState (0 = offline, 1 = online).</summary>
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate int GetFriendPersonaStateFn(IntPtr self, ulong steamIDFriend);
        internal static GetFriendPersonaStateFn GetFriendPersonaState;

        /// <summary>Returns a const char* (UTF-8) persona name for the friend.</summary>
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate IntPtr GetFriendPersonaNameFn(IntPtr self, ulong steamIDFriend);
        internal static GetFriendPersonaNameFn GetFriendPersonaName;

        /// <summary>Asks Steam to fetch persona/avatar data; names resolve asynchronously.</summary>
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate bool RequestUserInformationFn(IntPtr self, ulong steamIDUser, bool requireNameOnly);
        internal static RequestUserInformationFn RequestUserInformation;

        /// <summary>dialog is a NUL-terminated UTF-8 buffer (e.g. "friends", "chat").</summary>
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate void ActivateGameOverlayFn(IntPtr self, byte[] dialog);
        internal static ActivateGameOverlayFn ActivateGameOverlay;

        /// <summary>dialog is a NUL-terminated UTF-8 buffer (e.g. "steamid", "chat").</summary>
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate void ActivateGameOverlayToUserFn(IntPtr self, byte[] dialog, ulong steamID);
        internal static ActivateGameOverlayToUserFn ActivateGameOverlayToUser;

        /// <summary>connectString is a NUL-terminated UTF-8 buffer. Opens the Steam
        /// invite dialog; invitees get "Join Game" which delivers the string via
        /// GameRichPresenceJoinRequested_t or the +connect launch parameter.</summary>
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate void ActivateGameOverlayInviteDialogConnectStringFn(IntPtr self, byte[] connectString);
        internal static ActivateGameOverlayInviteDialogConnectStringFn ActivateGameOverlayInviteDialogConnectString;

        /// <summary>Opens the overlay on a store page. flag is EOverlayToStoreFlag
        /// (0 = none — plain store page).</summary>
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate void ActivateGameOverlayToStoreFn(IntPtr self, uint appID, int flag);
        internal static ActivateGameOverlayToStoreFn ActivateGameOverlayToStore;

        /// <summary>key/value are NUL-terminated UTF-8 buffers. Max 30 keys,
        /// key ≤ 64 chars, value ≤ 256 chars (k_cchMaxRichPresence*).</summary>
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate bool SetRichPresenceFn(IntPtr self, byte[] key, byte[] value);
        internal static SetRichPresenceFn SetRichPresence;

        /// <summary>Returns a const char* (UTF-8) rich-presence value, or null.</summary>
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate IntPtr GetFriendRichPresenceFn(IntPtr self, ulong steamIDFriend, byte[] key);
        internal static GetFriendRichPresenceFn GetFriendRichPresence;

        /// <summary>Marks a recently played-with user for the Steam "played with" list.</summary>
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate void SetPlayedWithFn(IntPtr self, ulong steamIDUserPlayedWith);
        internal static SetPlayedWithFn SetPlayedWith;

        /// <summary>
        /// Loads the Steamworks native library and binds every export the shim
        /// needs. Returns false (with <see cref="LoadError"/> set) when the DLL
        /// is absent — i.e. the game is not shipping Steam binaries — or when an
        /// export is missing because the DLL predates SDK 1.57.
        /// </summary>
        public static bool Load()
        {
            if (_lib != IntPtr.Zero) return true;
            if (_loadAttempted) return false;
            _loadAttempted = true;

            try
            {
                if (IsWindows)
                    _lib = LoadLibrary(IntPtr.Size == 8 ? "steam_api64.dll" : "steam_api.dll");
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                    _lib = dlopen("libsteam_api.dylib", RtldNow);
                else
                    _lib = dlopen("libsteam_api.so", RtldNow);
            }
            catch (Exception ex)
            {
                _loadError = ex.Message;
            }

            if (_lib == IntPtr.Zero)
            {
                if (_loadError == null)
                    _loadError = "steam_api native library not found next to the executable";
                return false;
            }

            try
            {
                RestartAppIfNecessary = Bind<RestartAppIfNecessaryFn>("SteamAPI_RestartAppIfNecessary");
                InitFlat = Bind<InitFlatFn>("SteamAPI_InitFlat");
                Shutdown = Bind<ShutdownFn>("SteamAPI_Shutdown");
                GetHSteamPipe = Bind<GetHSteamPipeFn>("SteamAPI_GetHSteamPipe");
                ManualDispatchInit = Bind<ManualDispatchInitFn>("SteamAPI_ManualDispatch_Init");
                ManualDispatchRunFrame = Bind<ManualDispatchRunFrameFn>("SteamAPI_ManualDispatch_RunFrame");
                ManualDispatchGetNextCallback = Bind<ManualDispatchGetNextCallbackFn>("SteamAPI_ManualDispatch_GetNextCallback");
                ManualDispatchFreeLastCallback = Bind<ManualDispatchFreeLastCallbackFn>("SteamAPI_ManualDispatch_FreeLastCallback");
                SteamUser = Bind<SteamUserFn>("SteamAPI_SteamUser_v023");
                SteamFriends = Bind<SteamFriendsFn>("SteamAPI_SteamFriends_v018");
                GetAuthTicketForWebApi = Bind<GetAuthTicketForWebApiFn>("SteamAPI_ISteamUser_GetAuthTicketForWebApi");
                GetSteamID = Bind<GetSteamIDFn>("SteamAPI_ISteamUser_GetSteamID");
                CancelAuthTicket = Bind<CancelAuthTicketFn>("SteamAPI_ISteamUser_CancelAuthTicket");
                GetPersonaName = Bind<GetPersonaNameFn>("SteamAPI_ISteamFriends_GetPersonaName");
                GetFriendCount = Bind<GetFriendCountFn>("SteamAPI_ISteamFriends_GetFriendCount");
                GetFriendByIndex = Bind<GetFriendByIndexFn>("SteamAPI_ISteamFriends_GetFriendByIndex");
                GetFriendRelationship = Bind<GetFriendRelationshipFn>("SteamAPI_ISteamFriends_GetFriendRelationship");
                GetFriendPersonaState = Bind<GetFriendPersonaStateFn>("SteamAPI_ISteamFriends_GetFriendPersonaState");
                GetFriendPersonaName = Bind<GetFriendPersonaNameFn>("SteamAPI_ISteamFriends_GetFriendPersonaName");
                RequestUserInformation = Bind<RequestUserInformationFn>("SteamAPI_ISteamFriends_RequestUserInformation");
                ActivateGameOverlay = Bind<ActivateGameOverlayFn>("SteamAPI_ISteamFriends_ActivateGameOverlay");
                ActivateGameOverlayToUser = Bind<ActivateGameOverlayToUserFn>("SteamAPI_ISteamFriends_ActivateGameOverlayToUser");
                ActivateGameOverlayInviteDialogConnectString = Bind<ActivateGameOverlayInviteDialogConnectStringFn>("SteamAPI_ISteamFriends_ActivateGameOverlayInviteDialogConnectString");
                ActivateGameOverlayToStore = Bind<ActivateGameOverlayToStoreFn>("SteamAPI_ISteamFriends_ActivateGameOverlayToStore");
                SetRichPresence = Bind<SetRichPresenceFn>("SteamAPI_ISteamFriends_SetRichPresence");
                GetFriendRichPresence = Bind<GetFriendRichPresenceFn>("SteamAPI_ISteamFriends_GetFriendRichPresence");
                SetPlayedWith = Bind<SetPlayedWithFn>("SteamAPI_ISteamFriends_SetPlayedWith");
            }
            catch (EntryPointNotFoundException ex)
            {
                _loadError = ex.Message;
                _lib = IntPtr.Zero; // treat as unavailable — the interface accessors are version-suffixed
                return false;
            }

            // Optional bindings — a missing export disables the feature, not
            // the whole API. Interface accessors are version-suffixed, so try
            // every suffix the shipped DLL may export.
            SteamAppsFn apps;
            _ = TryBind("SteamAPI_SteamApps_v009", out apps)
                || TryBind("SteamAPI_SteamApps_v008", out apps)
                || TryBind("SteamAPI_SteamApps_v007", out apps)
                || TryBind("SteamAPI_SteamApps_v006", out apps)
                || TryBind("SteamAPI_SteamApps_v005", out apps);
            SteamApps = apps;

            if (SteamApps != null)
                TryBind("SteamAPI_ISteamApps_GetCurrentGameLanguage", out GetCurrentGameLanguage);

            return true;
        }

        static bool TryBind<T>(string name, out T del) where T : class
        {
            del = null;
            if (_lib == IntPtr.Zero) return false;
            IntPtr p = IsWindows ? GetProcAddress(_lib, name) : dlsym(_lib, name);
            if (p == IntPtr.Zero) return false;
            del = Marshal.GetDelegateForFunctionPointer<T>(p);
            return true;
        }

        static T Bind<T>(string name) where T : class
        {
            IntPtr p = IsWindows ? GetProcAddress(_lib, name) : dlsym(_lib, name);
            if (p == IntPtr.Zero)
                throw new EntryPointNotFoundException(
                    "steam_api export missing: " + name + " (Steamworks SDK >= 1.57 required)");
            return Marshal.GetDelegateForFunctionPointer<T>(p);
        }
    }
}
