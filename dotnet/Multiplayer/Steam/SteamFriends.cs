using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Michitai.Multiplayer.Steam
{
    /// <summary>
    /// A single entry from the local user's Steam friend list.
    /// </summary>
    public sealed class SteamFriend
    {
        /// <summary>The friend's SteamID64.</summary>
        public ulong SteamId { get; internal set; }

        /// <summary>The friend's current persona name (empty until Steam resolves it).</summary>
        public string Name { get; internal set; }

        /// <summary>EPersonaState value — 0 offline, 1 online, 2 busy,
        /// 3 away, 4 snooze, 5 looking to trade, 6 looking to play.</summary>
        public int PersonaState { get; internal set; }

        /// <summary>EFriendRelationship value — 3 means a mutual friend.</summary>
        public int Relationship { get; internal set; }
    }

    /// <summary>
    /// ISteamFriends access layered on the SteamTicket shim: the friend list,
    /// rich presence, and the Steam overlay invite flow.
    ///
    /// Join flow: the host publishes a connect string via
    /// <see cref="SetJoinConnectString"/> (rich presence "connect" key) and
    /// opens <see cref="OpenInviteDialog"/>. Invited users either get a
    /// <see cref="SteamTicket.RichPresenceJoinRequested"/> event (game already
    /// running — pump <see cref="SteamTicket.RunFrame"/> on your main loop) or a
    /// "+connect &lt;string&gt;" launch parameter.
    /// </summary>
    public static class SteamFriends
    {
        /// <summary>EFriendFlags bitmask limiting to regular friends
        /// (k_EFriendFlagImmediate — excludes request/blocked entries).</summary>
        public const int FriendFlagImmediate = 0x04;

        /// <summary>EFriendFlags bitmask covering every relationship.</summary>
        public const int FriendFlagAll = unchecked((int)0xFFFF);

        /// <summary>True when the Steam API is initialized and these calls work.</summary>
        public static bool Available { get { return SteamTicket.Available; } }

        /// <summary>
        /// Lists the local user's Steam friends. Returns an empty list when
        /// Steam is unavailable. Pass includeAll=true to include pending
        /// requests and other non-immediate relationships.
        /// </summary>
        /// <param name="includeAll">Use the full EFriendFlags mask instead of immediate friends only.</param>
        /// <returns>Steam friends with persona state and relationship.</returns>
        public static List<SteamFriend> GetFriends(bool includeAll = false)
        {
            var friends = new List<SteamFriend>();
            if (!SteamTicket.Available) return friends;

            int flags = includeAll ? FriendFlagAll : FriendFlagImmediate;
            int count = SteamNative.GetFriendCount(SteamTicket.FriendsHandle, flags);
            for (int i = 0; i < count; i++)
            {
                ulong id = SteamNative.GetFriendByIndex(SteamTicket.FriendsHandle, i, flags);
                friends.Add(new SteamFriend
                {
                    SteamId = id,
                    Name = PtrToUtf8(SteamNative.GetFriendPersonaName(SteamTicket.FriendsHandle, id)) ?? "",
                    PersonaState = SteamNative.GetFriendPersonaState(SteamTicket.FriendsHandle, id),
                    Relationship = SteamNative.GetFriendRelationship(SteamTicket.FriendsHandle, id)
                });
            }
            return friends;
        }

        /// <summary>
        /// Asks Steam to fetch up-to-date persona data for a user; names in
        /// GetFriends() resolve asynchronously after this.
        /// </summary>
        /// <param name="steamId">The user's SteamID64.</param>
        /// <param name="requireNameOnly">True to only request the persona name.</param>
        /// <returns>True when data was already cached locally.</returns>
        public static bool RequestUserInformation(ulong steamId, bool requireNameOnly = false)
        {
            return SteamTicket.Available
                && SteamNative.RequestUserInformation(SteamTicket.FriendsHandle, steamId, requireNameOnly);
        }

        /// <summary>
        /// Sets a rich-presence key (max 30 keys; key ≤ 64 chars, value ≤ 256
        /// chars — longer values are rejected by Steam).
        /// </summary>
        /// <param name="key">Rich presence key (e.g. "connect", "status", "steam_display").</param>
        /// <param name="value">Value, or empty/null to clear the key.</param>
        /// <returns>True when Steam accepted the value.</returns>
        public static bool SetRichPresence(string key, string value)
        {
            if (!SteamTicket.Available || string.IsNullOrEmpty(key)) return false;
            return SteamNative.SetRichPresence(SteamTicket.FriendsHandle,
                Utf8(key), Utf8(value ?? ""));
        }

        /// <summary>
        /// Reads a rich-presence key for a friend (after their data resolves).
        /// </summary>
        /// <param name="steamId">The friend's SteamID64.</param>
        /// <param name="key">The rich presence key to read.</param>
        /// <returns>The value, or null when unset/unresolved.</returns>
        public static string GetFriendRichPresence(ulong steamId, string key)
        {
            if (!SteamTicket.Available || string.IsNullOrEmpty(key)) return null;
            return PtrToUtf8(SteamNative.GetFriendRichPresence(SteamTicket.FriendsHandle, steamId, Utf8(key)));
        }

        /// <summary>
        /// Publishes the lobby/room connect string as rich presence so Steam
        /// friends see "Join Game". Connect strings are opaque to Steam —
        /// format is up to the game (this SDK uses "mdl_&lt;id&gt;_&lt;token&gt;").
        /// </summary>
        /// <param name="connectString">Opaque join string (≤ 256 bytes).</param>
        /// <returns>True when published.</returns>
        public static bool SetJoinConnectString(string connectString)
            => SetRichPresence("connect", connectString);

        /// <summary>Clears the rich-presence connect string (lobby left/closed).</summary>
        public static void ClearJoinConnectString()
            => SetRichPresence("connect", "");

        /// <summary>
        /// Opens the Steam overlay invite dialog seeded with a connect string —
        /// no Steam lobby required. Invited users accept via "Join Game", which
        /// delivers the string through <see cref="SteamTicket.RichPresenceJoinRequested"/>
        /// or the "+connect" launch parameter.
        /// </summary>
        /// <param name="connectString">The same string published via SetJoinConnectString.</param>
        public static void OpenInviteDialog(string connectString)
        {
            if (!SteamTicket.Available || string.IsNullOrEmpty(connectString)) return;
            SteamNative.ActivateGameOverlayInviteDialogConnectString(
                SteamTicket.FriendsHandle, Utf8(connectString));
        }

        /// <summary>
        /// Opens the Steam overlay to a named dialog ("friends", "chat",
        /// "community", "settings", ...).
        /// </summary>
        /// <param name="dialog">Steam dialog name.</param>
        public static void OpenOverlay(string dialog = "friends")
        {
            if (!SteamTicket.Available) return;
            SteamNative.ActivateGameOverlay(SteamTicket.FriendsHandle, Utf8(dialog));
        }

        /// <summary>
        /// Opens the Steam overlay on the store page for an app — the UPDATE
        /// button target when no explicit update_url is configured.
        /// </summary>
        /// <param name="appId">Steam AppID whose store page to show.</param>
        public static void OpenStorePage(uint appId)
        {
            if (!SteamTicket.Available || appId == 0) return;
            SteamNative.ActivateGameOverlayToStore(SteamTicket.FriendsHandle, appId, 0);
        }

        /// <summary>
        /// Opens the Steam overlay on a user's profile/community page
        /// ("steamid" dialog) — handy for "view profile" on Steam friends.
        /// </summary>
        /// <param name="steamId">The user's SteamID64.</param>
        public static void OpenFriendProfile(ulong steamId)
        {
            if (!SteamTicket.Available) return;
            SteamNative.ActivateGameOverlayToUser(SteamTicket.FriendsHandle, Utf8("steamid"), steamId);
        }

        /// <summary>Marks a player as recently played-with on the Steam profile.</summary>
        /// <param name="steamId">The other player's SteamID64.</param>
        public static void SetPlayedWith(ulong steamId)
        {
            if (!SteamTicket.Available) return;
            SteamNative.SetPlayedWith(SteamTicket.FriendsHandle, steamId);
        }

        /// <summary>
        /// Extracts the "+connect" join string from launch arguments — the
        /// fresh-launch counterpart of <see cref="SteamTicket.RichPresenceJoinRequested"/>.
        /// </summary>
        /// <param name="args">Process args — pass null to read Environment.GetCommandLineArgs().</param>
        /// <returns>The connect string, or null when the game wasn't launched by an invite.</returns>
        public static string GetLaunchConnectString(string[] args = null)
        {
            args = args ?? Environment.GetCommandLineArgs();
            for (int i = 0; i < args.Length - 1; i++)
            {
                if (string.Equals(args[i], "+connect", StringComparison.OrdinalIgnoreCase)
                    || string.Equals(args[i], "-connect", StringComparison.OrdinalIgnoreCase))
                {
                    return args[i + 1];
                }
                // Steam also emits "+connect <value>" as a single token.
                if (args[i].StartsWith("+connect ", StringComparison.OrdinalIgnoreCase))
                {
                    return args[i].Substring(9).Trim();
                }
            }
            return null;
        }

        internal static byte[] Utf8(string s)
            => Encoding.UTF8.GetBytes((s ?? "") + "\0");

        internal static string PtrToUtf8(IntPtr p)
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
