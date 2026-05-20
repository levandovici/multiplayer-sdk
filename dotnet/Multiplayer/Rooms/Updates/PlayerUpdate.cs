using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Michitai.Multiplayer.Rooms.Updates
{
    /// <summary>
    /// Information about a player update received in a room.
    /// Contains update details, sender information, and data.
    /// </summary>
    /// <typeparam name="T">The type of update data.</typeparam>
    public class PlayerUpdate<T> where T : class, new()
    {
        /// <summary>
        /// The unique ID of the update.
        /// </summary>
        public string Update_id { get; set; } = string.Empty;

        /// <summary>
        /// The ID of the player who sent the update.
        /// </summary>
        public int From_player_id { get; set; }

        /// <summary>
        /// The type of update.
        /// </summary>
        public string Type { get; set; } = string.Empty;

        /// <summary>
        /// Timestamp when the update was created.
        /// </summary>
        public DateTimeOffset Created_at { get; set; }

        /// <summary>
        /// The update data deserialized into the specified type.
        /// </summary>
        public T? Data { get; set; }
    }
}
