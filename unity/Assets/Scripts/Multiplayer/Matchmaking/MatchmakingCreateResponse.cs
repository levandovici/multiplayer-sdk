using Michitai.Multiplayer.Errors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Michitai.Multiplayer.Matchmaking
{
    /// <summary>
    /// Response returned when a matchmaking lobby is successfully created.
    /// Contains the lobby ID and configuration details.
    /// </summary>
    [System.Serializable]
    public class MatchmakingCreateResponse : ApiResponse<EMatchmakingCreateError>
    {
        /// <summary>
        /// The unique ID of the created matchmaking lobby.
        /// </summary>
        public string matchmaking_id;

        /// <summary>
        /// The name of the matchmaking lobby.
        /// </summary>
        public string matchmaking_name;

        /// <summary>
        /// Maximum number of players allowed in the lobby.
        /// </summary>
        public int max_players;

        /// <summary>
        /// Whether the lobby must be full to start the game.
        /// </summary>
        public bool strict_full;

        /// <summary>
        /// Whether players must request to join and be approved by the host.
        /// </summary>
        public bool join_by_requests;

        /// <summary>
        /// Whether host switching is allowed in the resulting game room.
        /// </summary>
        public bool host_switch;

        /// <summary>
        /// Whether players can leave the resulting game room.
        /// </summary>
        public bool can_leave_room;

        /// <summary>
        /// Whether the resulting game room supports realtime communication.
        /// </summary>
        public bool realtime_room;

        /// <summary>
        /// Whether the player who created the lobby is the host.
        /// </summary>
        public bool is_host;
    }
}
