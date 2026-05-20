using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static Michitai.Multiplayer.Time.Time;

namespace Michitai.Multiplayer.Matchmaking
{
    /// <summary>
    /// Information about a matchmaking lobby in the list of available lobbies.
    /// Contains lobby configuration and basic player information.
    /// Uses Unity's JsonUtility for deserialization of rules.
    /// </summary>
    /// <typeparam name="T">The type to deserialize lobby rules into.</typeparam>
    [System.Serializable]
    public class MatchmakingLobby<T> where T : class, new()
    {
        [SerializeField]
        private string created_at;
        [SerializeField]
        private string last_heartbeat;
        [SerializeField]
        private string rules_json;      // Unity mode

        /// <summary>
        /// The unique ID of the matchmaking lobby.
        /// </summary>
        public string matchmaking_id;

        /// <summary>
        /// The name of the matchmaking lobby.
        /// </summary>
        public string matchmaking_name;

        /// <summary>
        /// The ID of the host player.
        /// </summary>
        public int host_player_id;

        /// <summary>
        /// Maximum number of players allowed.
        /// </summary>
        public int max_players;

        /// <summary>
        /// Whether the lobby must be full to start the game.
        /// </summary>
        public bool strict_full;

        /// <summary>
        /// Whether players must request to join and be approved.
        /// </summary>
        public bool join_by_requests;

        /// <summary>
        /// Whether host switching is allowed in the resulting room.
        /// </summary>
        public bool host_switch;

        /// <summary>
        /// Whether players can leave the resulting room.
        /// </summary>
        public bool can_leave_room;

        /// <summary>
        /// Whether the resulting room supports realtime communication.
        /// </summary>
        public bool realtime_room;

        /// <summary>
        /// Whether the lobby has a password set.
        /// </summary>
        public bool has_password;

        /// <summary>
        /// Current number of players in the lobby.
        /// </summary>
        public int current_players;

        /// <summary>
        /// The name of the host player.
        /// </summary>
        public string host_name;

        /// <summary>
        /// The lobby rules deserialized into the specified type.
        /// </summary>
        public T Rules
        {
            get
            {
                return JsonUtility.FromJson<T>(rules_json);
            }
        }

        /// <summary>
        /// Timestamp when the lobby was created.
        /// </summary>
        public DateTimeOffset? CreatedAt
        {
            get
            {
                return ParseUtc(created_at);
            }
        }

        /// <summary>
        /// Timestamp of the last heartbeat received from the lobby.
        /// </summary>
        public DateTimeOffset? LastHeartbeat
        {
            get
            {
                return ParseUtc(last_heartbeat);
            }
        }
    }
}
