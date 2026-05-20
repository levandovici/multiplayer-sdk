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
    /// Detailed information about a matchmaking lobby.
    /// Contains configuration, player counts, status, and rules.
    /// Uses Unity's JsonUtility for deserialization of rules.
    /// </summary>
    /// <typeparam name="T">The type to deserialize lobby rules into.</typeparam>
    [System.Serializable]
    public class MatchmakingInfo<T> where T : class, new()
    {
        [SerializeField]
        private string rules_json;           // Unity mode
        [SerializeField]
        private string joined_at;
        [SerializeField]
        private string last_heartbeat;
        [SerializeField]
        private string lobby_heartbeat;
        [SerializeField]
        private string started_at;

        /// <summary>
        /// The unique ID of the matchmaking lobby.
        /// </summary>
        public string matchmaking_id;

        /// <summary>
        /// The name of the matchmaking lobby.
        /// </summary>
        public string matchmaking_name;

        /// <summary>
        /// Whether the current player is the host.
        /// </summary>
        public bool is_host;

        /// <summary>
        /// Maximum number of players allowed.
        /// </summary>
        public int max_players;

        /// <summary>
        /// Current number of players in the lobby.
        /// </summary>
        public int current_players;

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
        /// Whether the lobby is currently online/active.
        /// </summary>
        public bool is_online;

        /// <summary>
        /// Whether the game has started from this lobby.
        /// </summary>
        public bool is_started;

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
        /// Timestamp when the current player joined the lobby.
        /// </summary>
        public DateTimeOffset? JoinedAt
        {
            get
            {
                return ParseUtc(joined_at);
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

        /// <summary>
        /// Timestamp of the lobby's last heartbeat.
        /// </summary>
        public DateTimeOffset? LobbyHeartbeat
        {
            get
            {
                return ParseUtc(lobby_heartbeat);
            }
        }

        /// <summary>
        /// Timestamp when the game started.
        /// </summary>
        public DateTimeOffset? StartedAt
        {
            get
            {
                return ParseUtc(started_at);
            }
        }
    }
}
