using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static Michitai.Multiplayer.Time.Time;

namespace Michitai.Multiplayer.Rooms.Actions
{
    /// <summary>
    /// Information about a pending action that needs to be completed by the host.
    /// Contains action details, sender information, and request data.
    /// Uses Unity's JsonUtility for deserialization of request data.
    /// </summary>
    /// <typeparam name="T">The type of request data.</typeparam>
    [System.Serializable]
    public class PendingAction<T> where T : class, new()
    {
        [SerializeField]
        private string request_data_json;    // Unity mode
        [SerializeField]
        private string created_at;

        /// <summary>
        /// The unique ID of the action.
        /// </summary>
        public string action_id;

        /// <summary>
        /// The ID of the player who submitted the action.
        /// </summary>
        public int player_id;

        /// <summary>
        /// The ID of the target player.
        /// </summary>
        public int target_id;

        /// <summary>
        /// The type of action.
        /// </summary>
        public string action_type;

        /// <summary>
        /// The name of the player who submitted the action.
        /// </summary>
        public string player_name;

        /// <summary>
        /// Whether the action was submitted by the host.
        /// </summary>
        public bool is_host;

        /// <summary>
        /// The request data deserialized into the specified type.
        /// </summary>
        public T RequestData
        {
            get
            {
                return JsonUtility.FromJson<T>(request_data_json);
            }
        }

        /// <summary>
        /// Timestamp when the action was created.
        /// </summary>
        public DateTimeOffset? CreatedAt
        {
            get
            {
                return ParseUtc(created_at);
            }
        }
    }
}
