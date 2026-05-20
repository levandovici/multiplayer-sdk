using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Michitai.Multiplayer.Matchmaking
{
    /// <summary>
    /// Request data for creating a new matchmaking lobby.
    /// Contains lobby configuration and serialized player data and rules.
    /// Uses Unity's JsonUtility for serialization.
    /// </summary>
    [System.Serializable]
    internal class MatchmakingCreateRequest
    {
        /// <summary>
        /// The name for the matchmaking lobby.
        /// </summary>
        public string matchmaking_name;

        /// <summary>
        /// Maximum number of players allowed.
        /// </summary>
        public int max_players;

        /// <summary>
        /// Whether the lobby must be full to start.
        /// </summary>
        public bool strict_full;

        /// <summary>
        /// Whether players must request to join.
        /// </summary>
        public bool join_by_requests;

        /// <summary>
        /// Whether host switching is allowed.
        /// </summary>
        public bool host_switch;

        /// <summary>
        /// Whether players can leave the resulting room.
        /// </summary>
        public bool can_leave_room;

        /// <summary>
        /// Whether the room supports realtime communication.
        /// </summary>
        public bool realtime_room;

        /// <summary>
        /// Password for the lobby.
        /// </summary>
        public string password;

        /// <summary>
        /// Serialized player data (Unity mode).
        /// </summary>
        public string player_data_json;

        /// <summary>
        /// Serialized lobby rules (Unity mode).
        /// </summary>
        public string rules_json;

        /// <summary>
        /// Initializes a new MatchmakingCreateRequest.
        /// </summary>
        /// <param name="matchmakingName">The name for the matchmaking lobby.</param>
        /// <param name="maxPlayers">Maximum number of players allowed.</param>
        /// <param name="strictFull">Whether the lobby must be full to start.</param>
        /// <param name="joinByRequests">Whether players must request to join.</param>
        /// <param name="hostSwitch">Whether host switching is allowed.</param>
        /// <param name="canLeaveRoom">Whether players can leave the resulting room.</param>
        /// <param name="realtimeRoom">Whether the room supports realtime communication.</param>
        /// <param name="password">Password for the lobby.</param>
        /// <param name="playerData">Serialized player data.</param>
        /// <param name="rulesJson">Serialized lobby rules.</param>
        public MatchmakingCreateRequest(string matchmakingName, int maxPlayers, bool strictFull,
            bool joinByRequests, bool hostSwitch, bool canLeaveRoom, bool realtimeRoom, string password, string playerData, string rulesJson)
        {
            this.matchmaking_name = matchmakingName;
            this.max_players = maxPlayers;
            this.strict_full = strictFull;
            this.join_by_requests = joinByRequests;
            this.host_switch = hostSwitch;
            this.can_leave_room = canLeaveRoom;
            this.realtime_room = realtimeRoom;
            this.password = password;
            this.player_data_json = playerData;
            this.rules_json = rulesJson;
        }
    }
}
