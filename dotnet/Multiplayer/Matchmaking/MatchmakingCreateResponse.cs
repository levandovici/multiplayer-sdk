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
    public class MatchmakingCreateResponse : ApiResponse<EMatchmakingCreateError>
    {
        /// <summary>
        /// The unique ID of the created matchmaking lobby.
        /// </summary>
        public string Matchmaking_id { get; set; } = string.Empty;

        /// <summary>
        /// The name of the matchmaking lobby.
        /// </summary>
        public string Matchmaking_name { get; set; } = string.Empty;

        /// <summary>
        /// Maximum number of players allowed in the lobby.
        /// </summary>
        public int Max_players { get; set; }

        /// <summary>
        /// Whether the lobby must be full to start the game.
        /// </summary>
        public bool Strict_full { get; set; }

        /// <summary>
        /// Whether players must request to join and be approved by the host.
        /// </summary>
        public bool Join_by_requests { get; set; }

        /// <summary>
        /// Whether host switching is allowed in the resulting game room.
        /// </summary>
        public bool Host_switch { get; set; }

        /// <summary>
        /// Whether players can leave the resulting game room.
        /// </summary>
        public bool Can_leave_room { get; set; }

        /// <summary>
        /// Whether the resulting game room supports realtime communication.
        /// </summary>
        public bool Realtime_room { get; set; }

        /// <summary>
        /// Whether the player who created the lobby is the host.
        /// </summary>
        public bool Is_host { get; set; }
    }
}
