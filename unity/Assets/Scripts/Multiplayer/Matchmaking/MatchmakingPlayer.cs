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
    /// Information about a player in a matchmaking lobby.
    /// Contains player details and connection status.
    /// Uses Unity's JsonUtility for deserialization of player data.
    /// </summary>
    /// <typeparam name="T">The type to deserialize player data into.</typeparam>
    [System.Serializable]
    public class MatchmakingPlayer<T> where T : class, new()
    {
        [SerializeField]
        private string joined_at;
        [SerializeField]
        private string last_heartbeat;
        [SerializeField]
        private string player_data_json;        // Unity mode

        /// <summary>
        /// The player's unique ID.
        /// </summary>
        public int player_id;

        /// <summary>
        /// Whether this player is the local player.
        /// </summary>
        public bool is_local;

        /// <summary>
        /// Whether the player is currently online.
        /// </summary>
        public bool is_online;

        /// <summary>
        /// The player's display name.
        /// </summary>
        public string player_name;

        /// <summary>
        /// Seconds since the player's last heartbeat.
        /// </summary>
        public int seconds_since_heartbeat;

        /// <summary>
        /// Whether the player is the lobby host.
        /// </summary>
        public bool is_host;

        /// <summary>
        /// The player's custom data deserialized into the specified type.
        /// </summary>
        public T PlayerData
        {
            get
            {
                return JsonUtility.FromJson<T>(player_data_json);
            }
        }

        /// <summary>
        /// Timestamp when the player joined the lobby.
        /// </summary>
        public DateTimeOffset? JoinedAt
        {
            get
            {
                return ParseUtc(joined_at);
            }
        }

        /// <summary>
        /// Timestamp of the player's last heartbeat.
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
