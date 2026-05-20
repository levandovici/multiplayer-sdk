using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static Michitai.Multiplayer.Time.Time;

namespace Michitai.Multiplayer.Rooms
{
    /// <summary>
    /// Detailed information about the current game room.
    /// Contains room configuration, player counts, status, and rules.
    /// Uses Unity's JsonUtility for deserialization of rules.
    /// </summary>
    /// <typeparam name="T">The type to deserialize room rules into.</typeparam>
    [System.Serializable]
    public class CurrentRoomInfo<T> where T : class, new()
    {
        [SerializeField]
        private string joined_at;
        [SerializeField]
        private string last_heartbeat;
        [SerializeField]
        private string room_created_at;
        [SerializeField]
        private string room_last_activity;
        [SerializeField]
        private string rules_json;          // Unity mode

        /// <summary>
        /// The unique ID of the room.
        /// </summary>
        public string room_id;

        /// <summary>
        /// The name of the room.
        /// </summary>
        public string room_name;

        /// <summary>
        /// Whether the current player is the host.
        /// </summary>
        public bool is_host;

        /// <summary>
        /// Whether the room is currently online/active.
        /// </summary>
        public bool is_online;

        /// <summary>
        /// Maximum number of players allowed.
        /// </summary>
        public int max_players;

        /// <summary>
        /// Current number of players in the room.
        /// </summary>
        public int current_players;

        /// <summary>
        /// Whether the room has a password set.
        /// </summary>
        public bool has_password;

        /// <summary>
        /// Whether host switching is allowed.
        /// </summary>
        public bool host_switch;

        /// <summary>
        /// Whether players can leave the room.
        /// </summary>
        public bool can_leave;

        /// <summary>
        /// Whether the room supports realtime communication.
        /// </summary>
        public bool realtime;

        /// <summary>
        /// Whether the room is currently active.
        /// </summary>
        public bool is_active;

        /// <summary>
        /// The current player's name.
        /// </summary>
        public string player_name;

        /// <summary>
        /// The room rules deserialized into the specified type.
        /// </summary>
        public T Rules
        {
            get
            {
                return JsonUtility.FromJson<T>(rules_json);
            }
        }

        /// <summary>
        /// Timestamp when the current player joined the room.
        /// </summary>
        public DateTimeOffset? JoinedAt
        {
            get
            {
                return ParseUtc(joined_at);
            }
        }

        /// <summary>
        /// Timestamp of the last heartbeat from the current player.
        /// </summary>
        public DateTimeOffset? LastHeartbeat
        {
            get
            {
                return ParseUtc(last_heartbeat);
            }
        }

        /// <summary>
        /// Timestamp when the room was created.
        /// </summary>
        public DateTimeOffset? RoomCreatedAt
        {
            get
            {
                return ParseUtc(room_created_at);
            }
        }

        /// <summary>
        /// Timestamp of the last activity in the room.
        /// </summary>
        public DateTimeOffset? RoomLastActivity
        {
            get
            {
                return ParseUtc(room_last_activity);
            }
        }
    }
}
