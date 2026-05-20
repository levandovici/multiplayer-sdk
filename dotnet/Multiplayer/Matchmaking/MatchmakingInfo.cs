using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Michitai.Multiplayer.Matchmaking
{
    /// <summary>
    /// Detailed information about a matchmaking lobby.
    /// Contains configuration, player counts, status, and rules.
    /// </summary>
    /// <typeparam name="T">The type to deserialize lobby rules into.</typeparam>
    public class MatchmakingInfo<T> where T : class, new()
    {
        /// <summary>
        /// The unique ID of the matchmaking lobby.
        /// </summary>
        public string Matchmaking_id { get; set; } = string.Empty;

        /// <summary>
        /// The name of the matchmaking lobby.
        /// </summary>
        public string Matchmaking_name { get; set; } = string.Empty;

        /// <summary>
        /// Whether the current player is the host.
        /// </summary>
        public bool Is_host { get; set; }

        /// <summary>
        /// Maximum number of players allowed.
        /// </summary>
        public int Max_players { get; set; }

        /// <summary>
        /// Current number of players in the lobby.
        /// </summary>
        public int Current_players { get; set; }

        /// <summary>
        /// Whether the lobby must be full to start the game.
        /// </summary>
        public bool Strict_full { get; set; }

        /// <summary>
        /// Whether players must request to join and be approved.
        /// </summary>
        public bool Join_by_requests { get; set; }

        /// <summary>
        /// Whether host switching is allowed in the resulting room.
        /// </summary>
        public bool Host_switch { get; set; }

        /// <summary>
        /// Whether players can leave the resulting room.
        /// </summary>
        public bool Can_leave_room { get; set; }

        /// <summary>
        /// Whether the resulting room supports realtime communication.
        /// </summary>
        public bool Realtime_room { get; set; }

        /// <summary>
        /// Whether the lobby has a password set.
        /// </summary>
        public bool Has_password { get; set; }

        /// <summary>
        /// Timestamp when the current player joined the lobby.
        /// </summary>
        public DateTimeOffset Joined_at { get; set; }

        /// <summary>
        /// Whether the lobby is currently online/active.
        /// </summary>
        public bool Is_online { get; set; }

        /// <summary>
        /// Timestamp of the last heartbeat received from the lobby.
        /// </summary>
        public DateTimeOffset Last_heartbeat { get; set; }

        /// <summary>
        /// Timestamp of the lobby's last heartbeat.
        /// </summary>
        public DateTimeOffset Lobby_heartbeat { get; set; }

        /// <summary>
        /// Whether the game has started from this lobby.
        /// </summary>
        public bool Is_started { get; set; }

        /// <summary>
        /// Timestamp when the game started (null if not started).
        /// </summary>
        public DateTimeOffset? Started_at { get; set; }

        /// <summary>
        /// The lobby rules deserialized into the specified type.
        /// </summary>
        public T? Rules { get; set; }
    }
}
