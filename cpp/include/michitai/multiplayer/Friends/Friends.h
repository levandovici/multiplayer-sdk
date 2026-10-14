#pragma once

#include "../Client.h"
#include "../ApiResponse.h"
#include "../Types.h"
#include <string>
#include <vector>
#include <optional>

namespace michitai {
namespace multiplayer {
namespace friends {

// ====================== ENDPOINTS ======================

namespace Endpoints {
    constexpr const char* FriendsList = "friends.php/list";
    constexpr const char* FriendsLookup = "friends.php/lookup";
    constexpr const char* FriendsRequest = "friends.php/request";
    constexpr const char* FriendsAccept = "friends.php/accept";
    constexpr const char* FriendsDecline = "friends.php/decline";
    constexpr const char* FriendsRemove = "friends.php/remove";
    constexpr const char* FriendsSteam = "friends.php/steam";
    constexpr const char* FriendsInvite = "friends.php/invite";
    constexpr const char* FriendsInvites = "friends.php/invites";
    constexpr const char* FriendsInvitesAccept = "friends.php/invites/accept";
    constexpr const char* FriendsInvitesDecline = "friends.php/invites/decline";
}

// ====================== TYPES ======================

/// Public information about a player in a friends context — never contains
/// tokens or player data.
struct FriendInfo {
    int playerId = 0;
    std::string playerName;
    std::string playerTag;   // public friend tag ("#XXXXXXXX")
    bool isOnline = false;
    std::string steamId;
    std::string relationship; // "friend", "incoming", "outgoing" or empty
    std::string requestedAt;

    static FriendInfo fromJson(const nlohmann::json& j) {
        FriendInfo info;
        info.playerId = j.value("player_id", 0);
        info.playerName = j.value("player_name", "");
        info.isOnline = j.value("is_online", false);
        info.relationship = j.value("relationship", "");
        info.requestedAt = j.value("requested_at", "");
        if (j.contains("player_tag") && !j["player_tag"].is_null()) {
            info.playerTag = j["player_tag"].get<std::string>();
        }
        if (j.contains("steam_id") && !j["steam_id"].is_null()) {
            info.steamId = j["steam_id"].get<std::string>();
        }
        return info;
    }
};

/// A pending lobby or room invitation received from a friend.
struct FriendInvite {
    std::string inviteId;
    std::string kind;  // "matchmaking" or "room"
    std::string name;  // lobby or room display name
    std::optional<FriendInfo> fromPlayer;
    std::string createdAt;

    static FriendInvite fromJson(const nlohmann::json& j) {
        FriendInvite invite;
        invite.inviteId = j.value("invite_id", "");
        invite.kind = j.value("kind", "");
        invite.name = j.value("name", "");
        invite.createdAt = j.value("created_at", "");
        if (j.contains("from_player") && !j["from_player"].is_null()) {
            invite.fromPlayer = FriendInfo::fromJson(j["from_player"]);
        }
        return invite;
    }
};

// ====================== RESPONSE TYPES ======================

/// Friends list, pending requests, and the local player's own tag.
struct FriendListResponse : public ApiResponse {
    std::string playerTag;
    std::vector<FriendInfo> friends;
    std::vector<FriendInfo> incoming;
    std::vector<FriendInfo> outgoing;

    static FriendListResponse fromJson(const nlohmann::json& j) {
        FriendListResponse response;
        response.success = j.value("success", false);
        response.error = j.value("error", "");
        response.playerTag = j.value("player_tag", "");
        if (j.contains("friends") && j["friends"].is_array()) {
            for (const auto& p : j["friends"]) response.friends.push_back(FriendInfo::fromJson(p));
        }
        if (j.contains("incoming") && j["incoming"].is_array()) {
            for (const auto& p : j["incoming"]) response.incoming.push_back(FriendInfo::fromJson(p));
        }
        if (j.contains("outgoing") && j["outgoing"].is_array()) {
            for (const auto& p : j["outgoing"]) response.outgoing.push_back(FriendInfo::fromJson(p));
        }
        return response;
    }
};

/// Result of a player-tag lookup.
struct FriendLookupResponse : public ApiResponse {
    std::optional<FriendInfo> player;
    std::string relationship;

    static FriendLookupResponse fromJson(const nlohmann::json& j) {
        FriendLookupResponse response;
        response.success = j.value("success", false);
        response.error = j.value("error", "");
        if (j.contains("player") && !j["player"].is_null()) {
            response.player = FriendInfo::fromJson(j["player"]);
        }
        response.relationship = j.value("relationship", "");
        return response;
    }
};

/// Result of a friend request/accept/decline/remove operation.
struct FriendStatusResponse : public ApiResponse {
    std::string status; // "pending", "accepted", "declined" or "removed"
    int playerId = 0;

    static FriendStatusResponse fromJson(const nlohmann::json& j) {
        FriendStatusResponse response;
        response.success = j.value("success", false);
        response.error = j.value("error", "");
        response.status = j.value("status", "");
        response.playerId = j.value("player_id", 0);
        return response;
    }
};

/// Result of sending a lobby/room invite.
struct FriendInviteResponse : public ApiResponse {
    std::string inviteId;
    std::string status;

    static FriendInviteResponse fromJson(const nlohmann::json& j) {
        FriendInviteResponse response;
        response.success = j.value("success", false);
        response.error = j.value("error", "");
        response.inviteId = j.value("invite_id", "");
        response.status = j.value("status", "");
        return response;
    }
};

/// Pending lobby/room invites for the local player.
struct FriendInvitesResponse : public ApiResponse {
    std::vector<FriendInvite> invites;

    static FriendInvitesResponse fromJson(const nlohmann::json& j) {
        FriendInvitesResponse response;
        response.success = j.value("success", false);
        response.error = j.value("error", "");
        if (j.contains("invites") && j["invites"].is_array()) {
            for (const auto& i : j["invites"]) response.invites.push_back(FriendInvite::fromJson(i));
        }
        return response;
    }
};

/// Result of accepting or declining an invite. On accept, matchmakingId or
/// roomId identifies the joined target.
struct FriendInviteAcceptResponse : public ApiResponse {
    std::string status;
    std::string inviteId;
    std::string matchmakingId;
    std::string roomId;

    static FriendInviteAcceptResponse fromJson(const nlohmann::json& j) {
        FriendInviteAcceptResponse response;
        response.success = j.value("success", false);
        response.error = j.value("error", "");
        response.status = j.value("status", "");
        response.inviteId = j.value("invite_id", "");
        response.matchmakingId = j.value("matchmaking_id", "");
        response.roomId = j.value("room_id", "");
        return response;
    }
};

/// Players linked to the SteamIDs passed to the lookup.
struct SteamFriendsLookupResponse : public ApiResponse {
    std::vector<FriendInfo> players;

    static SteamFriendsLookupResponse fromJson(const nlohmann::json& j) {
        SteamFriendsLookupResponse response;
        response.success = j.value("success", false);
        response.error = j.value("error", "");
        if (j.contains("players") && j["players"].is_array()) {
            for (const auto& p : j["players"]) response.players.push_back(FriendInfo::fromJson(p));
        }
        return response;
    }
};

// ====================== FRIENDS CLASS ======================

/// Provides methods for the friends system: tag lookups, friend requests,
/// Steam-friend import, and lobby/room invitations. Invited friends join
/// without the lobby password — the invite itself is the server's
/// authorization.
class Friends {
public:
    /// Gets the local player's friends list, pending requests, and own tag.
    static FriendListResponse getFriends(Client& client,
                                         const std::string& playerToken) {
        return client.get<FriendListResponse>(
            client.url(Endpoints::FriendsList), playerToken
        );
    }

    /// Looks up a player by friend tag ("#XXXXXXXX" or the 8-char body).
    static FriendLookupResponse lookupPlayerByTag(Client& client,
                                                   const std::string& playerToken,
                                                   const std::string& playerTag) {
        return client.post<FriendLookupResponse>(
            client.url(Endpoints::FriendsLookup),
            {{"player_tag", playerTag}}, playerToken
        );
    }

    /// Sends a friend request by tag. If the target already sent one, it is
    /// accepted automatically (status "accepted").
    static FriendStatusResponse sendFriendRequestByTag(Client& client,
                                                        const std::string& playerToken,
                                                        const std::string& playerTag) {
        return client.post<FriendStatusResponse>(
            client.url(Endpoints::FriendsRequest),
            {{"player_tag", playerTag}}, playerToken
        );
    }

    /// Sends a friend request by player ID (e.g. from a Steam-friend lookup).
    static FriendStatusResponse sendFriendRequest(Client& client,
                                                   const std::string& playerToken,
                                                   int playerId) {
        return client.post<FriendStatusResponse>(
            client.url(Endpoints::FriendsRequest),
            {{"player_id", playerId}}, playerToken
        );
    }

    /// Accepts a pending friend request.
    static FriendStatusResponse acceptFriendRequest(Client& client,
                                                     const std::string& playerToken,
                                                     int playerId) {
        return client.post<FriendStatusResponse>(
            client.url(Endpoints::FriendsAccept),
            {{"player_id", playerId}}, playerToken
        );
    }

    /// Declines a pending friend request.
    static FriendStatusResponse declineFriendRequest(Client& client,
                                                      const std::string& playerToken,
                                                      int playerId) {
        return client.post<FriendStatusResponse>(
            client.url(Endpoints::FriendsDecline),
            {{"player_id", playerId}}, playerToken
        );
    }

    /// Removes a friend.
    static FriendStatusResponse removeFriend(Client& client,
                                              const std::string& playerToken,
                                              int playerId) {
        return client.post<FriendStatusResponse>(
            client.url(Endpoints::FriendsRemove),
            {{"player_id", playerId}}, playerToken
        );
    }

    /// Looks up game accounts linked to the given SteamID64s (max 100) — for
    /// importing Steam friends into the backend friends list.
    static SteamFriendsLookupResponse lookupSteamFriends(Client& client,
                                                          const std::string& playerToken,
                                                          const std::vector<std::string>& steamIds) {
        return client.post<SteamFriendsLookupResponse>(
            client.url(Endpoints::FriendsSteam),
            {{"steam_ids", steamIds}}, playerToken
        );
    }

    /// Invites a friend into the local player's current lobby or room. The
    /// invitee bypasses password and host approval.
    static FriendInviteResponse inviteFriend(Client& client,
                                              const std::string& playerToken,
                                              int playerId) {
        return client.post<FriendInviteResponse>(
            client.url(Endpoints::FriendsInvite),
            {{"player_id", playerId}}, playerToken
        );
    }

    /// Lists pending lobby/room invites for the local player.
    static FriendInvitesResponse getInvites(Client& client,
                                             const std::string& playerToken) {
        return client.get<FriendInvitesResponse>(
            client.url(Endpoints::FriendsInvites), playerToken
        );
    }

    /// Accepts a lobby/room invite and joins it directly — no password or
    /// host approval required. Returns the joined matchmakingId or roomId.
    template<typename T = nlohmann::json>
    static FriendInviteAcceptResponse acceptInvite(Client& client,
                                                    const std::string& playerToken,
                                                    const std::string& inviteId,
                                                    const std::optional<T>& playerData = std::nullopt) {
        nlohmann::json body = nlohmann::json::object();
        body["invite_id"] = inviteId;
        if (playerData.has_value()) {
            if constexpr (std::is_same_v<T, nlohmann::json>) {
                body["player_data"] = playerData.value();
            } else {
                body["player_data"] = nlohmann::json(playerData.value());
            }
        }
        return client.post<FriendInviteAcceptResponse>(
            client.url(Endpoints::FriendsInvitesAccept),
            body, playerToken
        );
    }

    /// Declines a lobby/room invite.
    static FriendInviteAcceptResponse declineInvite(Client& client,
                                                     const std::string& playerToken,
                                                     const std::string& inviteId) {
        return client.post<FriendInviteAcceptResponse>(
            client.url(Endpoints::FriendsInvitesDecline),
            {{"invite_id", inviteId}}, playerToken
        );
    }
};

} // namespace friends
} // namespace multiplayer
} // namespace michitai
