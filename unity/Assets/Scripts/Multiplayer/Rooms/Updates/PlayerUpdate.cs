using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static Michitai.Multiplayer.Time.Time;

namespace Michitai.Multiplayer.Rooms.Updates
{
    /// <summary>
    /// Information about a player update received in a room.
    /// Contains update details, sender information, and data.
    /// </summary>
    [System.Serializable]
    public class PlayerUpdate
    {
        [SerializeField]
        private string created_at;

        /// <summary>
        /// The unique ID of the update.
        /// </summary>
        public string update_id;

        /// <summary>
        /// The ID of the player who sent the update.
        /// </summary>
        public int from_player_id;

        /// <summary>
        /// The type of update.
        /// </summary>
        public string type;

        /// <summary>
        /// The update data as JSON string.
        /// </summary>
        public string data_json;    // Unity mode

        /// <summary>
        /// Timestamp when the update was created.
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
