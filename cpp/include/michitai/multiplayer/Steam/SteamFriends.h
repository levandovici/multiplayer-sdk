#pragma once

/// Steam friends, rich presence and overlay-invite helpers for the C++ SDK —
/// thin wrappers over the real ISteamFriends interface, layered on SteamTicket.
///
/// Join flow: the host publishes a connect string via SetJoinConnectString
/// (rich presence "connect" key) and opens OpenInviteDialog. Invited users
/// either get a GameRichPresenceJoinRequested_t callback (game already
/// running — listen via SteamTicket::OnRichPresenceJoin or your own
/// STEAM_CALLBACK) or a "+connect <string>" launch parameter.
///
/// Same requirements as SteamTicket.h — Steamworks SDK >= 1.57 on the include
/// path, steam_api64.dll/lib shipped, Steam client running.

#include "SteamTicket.h"
#include <steam/isteamfriends.h>
#include <cstring>
#include <string>
#include <vector>

namespace michitai::multiplayer::steam {

/// A single entry from the local user's Steam friend list.
struct SteamFriendInfo {
    uint64 steamId = 0;        // SteamID64
    std::string name;          // persona name (may be empty until resolved)
    EPersonaState personaState = k_EPersonaStateOffline;
    EFriendRelationship relationship = k_EFriendRelationshipNone;
};

/// Lists the local user's Steam friends. Pass flags = k_EFriendFlagImmediate
/// (default) for mutual friends only, or k_EFriendFlagAll to include pending
/// requests and blocked entries.
inline std::vector<SteamFriendInfo> GetSteamFriends(
    EFriendFlags flags = k_EFriendFlagImmediate)
{
    std::vector<SteamFriendInfo> out;
    ISteamFriends* friends = SteamFriends();
    if (friends == nullptr) return out;

    const int count = friends->GetFriendCount(flags);
    out.reserve(static_cast<size_t>(count));
    for (int i = 0; i < count; ++i)
    {
        const CSteamID id = friends->GetFriendByIndex(i, flags);
        const char* name = friends->GetFriendPersonaName(id);
        SteamFriendInfo info;
        info.steamId = id.ConvertToUint64();
        if (name != nullptr) info.name = name;
        info.personaState = friends->GetFriendPersonaState(id);
        info.relationship = friends->GetFriendRelationship(id);
        out.push_back(std::move(info));
    }
    return out;
}

/// Sets a rich-presence key (max 30 keys; key <= 64 chars, value <= 256 chars
/// — longer values are rejected by Steam). Pass an empty value to clear.
inline bool SetRichPresence(const std::string& key, const std::string& value)
{
    ISteamFriends* friends = SteamFriends();
    return friends != nullptr
        && friends->SetRichPresence(key.c_str(), value.c_str());
}

/// Reads a rich-presence key for a friend (after their data resolves).
/// Empty string means unset/unresolved.
inline std::string GetFriendRichPresence(uint64 steamId, const std::string& key)
{
    ISteamFriends* friends = SteamFriends();
    if (friends == nullptr) return {};
    const char* value = friends->GetFriendRichPresence(
        CSteamID(steamId), key.c_str());
    return value != nullptr ? std::string(value) : std::string();
}

/// Publishes the lobby/room connect string as rich presence so Steam friends
/// see "Join Game". Connect strings are opaque to Steam — this SDK uses
/// "mdl_<id>_<token>".
inline bool SetJoinConnectString(const std::string& connectString)
{
    return SetRichPresence("connect", connectString);
}

/// Clears the rich-presence connect string (lobby left/closed).
inline void ClearJoinConnectString()
{
    SetRichPresence("connect", "");
}

/// Opens the Steam overlay invite dialog seeded with a connect string — no
/// Steam lobby required. Invited users accept via "Join Game", which delivers
/// the string through GameRichPresenceJoinRequested_t (see
/// SteamTicket::OnRichPresenceJoin) or the "+connect" launch parameter.
inline void OpenInviteDialog(const std::string& connectString)
{
    ISteamFriends* friends = SteamFriends();
    if (friends == nullptr || connectString.empty()) return;
    friends->ActivateGameOverlayInviteDialogConnectString(connectString.c_str());
}

/// Opens the Steam overlay to a named dialog ("friends", "chat",
/// "community", "settings", ...).
inline void OpenOverlay(const char* dialog = "friends")
{
    ISteamFriends* friends = SteamFriends();
    if (friends != nullptr) friends->ActivateGameOverlay(dialog);
}

/// Opens the Steam overlay on a user's profile/community page — handy for
/// "view profile" on Steam friends.
inline void OpenFriendProfile(uint64 steamId)
{
    ISteamFriends* friends = SteamFriends();
    if (friends != nullptr)
        friends->ActivateGameOverlayToUser("steamid", CSteamID(steamId));
}

/// Marks a player as recently played-with on the Steam profile.
inline void SetPlayedWith(uint64 steamId)
{
    ISteamFriends* friends = SteamFriends();
    if (friends != nullptr)
        friends->SetPlayedWith(CSteamID(steamId));
}

/// Extracts the "+connect" join string from launch arguments — the
/// fresh-launch counterpart of GameRichPresenceJoinRequested_t.
/// Pass argv/argc, or rely on your platform's own argv retrieval.
inline std::string GetLaunchConnectString(int argc, const char* const* argv)
{
    if (argv == nullptr) return {};
    for (int i = 0; i + 1 < argc; ++i)
    {
        const char* arg = argv[i];
        if (arg == nullptr) continue;
        if (std::strcmp(arg, "+connect") == 0 || std::strcmp(arg, "-connect") == 0)
            return argv[i + 1] != nullptr ? std::string(argv[i + 1]) : std::string();
        // Steam also emits "+connect <value>" as a single token.
        if (std::strncmp(arg, "+connect ", 9) == 0)
            return std::string(arg + 9);
    }
    return {};
}

/// Add this where you process Steam callbacks (after SteamAPI_RunCallbacks):
///
///   STEAM_CALLBACK(YourClass, OnJoinRequested, GameRichPresenceJoinRequested_t);
///   void YourClass::OnJoinRequested(GameRichPresenceJoinRequested_t* p) {
///       std::string connect = p->m_rgchConnect;          // "mdl_<id>_<token>"
///       uint64 inviter = p->m_steamIDFriend.ConvertToUint64();
///       ... join the lobby/room via the API ...
///   }

} // namespace michitai::multiplayer::steam
