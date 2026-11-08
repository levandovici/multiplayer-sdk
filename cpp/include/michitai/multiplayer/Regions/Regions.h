#pragma once

#include "../Client.h"
#include "../ApiResponse.h"
#include <string>
#include <vector>
#include <optional>
#include <chrono>

namespace michitai {
namespace multiplayer {
namespace regions {

// ====================== ENDPOINTS ======================

namespace Endpoints {
    constexpr const char* Discover = "discover.php";
}

// ====================== RESPONSE TYPES ======================

/// Realtime (WS/UDP) endpoint advertised by a service node
struct RealtimeEndpoint {
    std::string host;
    int port = 443;
    std::string protocol = "wss";
    std::optional<int> udp_port;

    static RealtimeEndpoint fromJson(const nlohmann::json& j) {
        RealtimeEndpoint rt;
        rt.host = j.value("host", "");
        rt.port = j.value("port", 443);
        rt.protocol = j.value("protocol", "wss");
        if (j.contains("udp_port") && !j["udp_port"].is_null()) {
            rt.udp_port = j["udp_port"].get<int>();
        }
        return rt;
    }
};

/// One enabled API region from the discovery directory
struct ServiceNode {
    std::string region;
    std::string label;
    std::string api_url;
    std::optional<RealtimeEndpoint> realtime;

    static ServiceNode fromJson(const nlohmann::json& j) {
        ServiceNode node;
        node.region = j.value("region", "");
        node.label = j.value("label", "");
        node.api_url = j.value("api_url", "");
        if (j.contains("realtime") && j["realtime"].is_object()) {
            node.realtime = RealtimeEndpoint::fromJson(j["realtime"]);
        }
        return node;
    }
};

/// Enabled service nodes plus the server's geo suggestion
struct DiscoverResponse : public ApiResponse {
    std::vector<ServiceNode> nodes;
    std::optional<std::string> suggested_region;

    static DiscoverResponse fromJson(const nlohmann::json& j) {
        DiscoverResponse response;
        response.success = j.value("success", false);
        response.error = j.value("error", "");
        if (j.contains("nodes") && j["nodes"].is_array()) {
            for (const auto& n : j["nodes"]) {
                response.nodes.push_back(ServiceNode::fromJson(n));
            }
        }
        if (j.contains("suggested_region") && j["suggested_region"].is_string()) {
            response.suggested_region = j["suggested_region"].get<std::string>();
        }
        return response;
    }
};

/// Result of discover.php?api_key=… — the node a game is pinned to
struct GameNodeResponse : public ApiResponse {
    std::string region;
    std::optional<ServiceNode> node;

    static GameNodeResponse fromJson(const nlohmann::json& j) {
        GameNodeResponse response;
        response.success = j.value("success", false);
        response.error = j.value("error", "");
        response.region = j.value("region", "");
        if (j.contains("node") && j["node"].is_object()) {
            response.node = ServiceNode::fromJson(j["node"]);
        }
        return response;
    }
};

// ====================== API METHODS ======================

/// Region discovery & game resolution.
///
/// discover() returns every enabled service node plus the server's geo
/// guess. A game is pinned to exactly one node — chosen when the API key
/// is created; resolveGame() asks the directory which node hosts it.
/// Player accounts live on that node only and never roam — pin the
/// client's base URL to it.
class Regions {
public:
    /// Fetch the enabled service-node directory.
    /// @param client The API client instance
    /// @return Node list plus the server's suggested_region hint
    static DiscoverResponse discover(Client& client) {
        return client.get<DiscoverResponse>(client.url(Endpoints::Discover));
    }

    /// Resolve which node hosts a game.
    ///
    /// Point every subsequent REST and realtime call at the returned
    /// node's api_url: the game's players, rooms and leaderboards exist
    /// only there, and other nodes answer 421 with a home_api_url hint
    /// for the same resolution.
    /// @param client The API client instance (queries the directory — use
    ///        the central base URL)
    /// @param apiKey The game's public api_key
    /// @return Region code plus the pinned node
    static GameNodeResponse resolveGame(Client& client, const std::string& apiKey) {
        // The client attaches X-Api-Token on every request — POST with no
        // query string; the key never travels in a URL.
        return client.post<GameNodeResponse>(client.url(Endpoints::Discover), apiKey);
    }
};

} // namespace regions
} // namespace multiplayer
} // namespace michitai
