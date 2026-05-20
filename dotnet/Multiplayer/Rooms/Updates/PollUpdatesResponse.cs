using Michitai.Multiplayer.Errors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Michitai.Multiplayer.Rooms.Updates
{
    /// <summary>
    /// Response containing player updates that were targeted to the polling player.
    /// </summary>
    /// <typeparam name="T">The type to deserialize update data into.</typeparam>
    public class PollUpdatesResponse<T> : ApiResponse<ERoomUpdatesPollError> where T : class, new()
    {
        /// <summary>
        /// List of player updates received.
        /// </summary>
        public List<PlayerUpdate<T>> Updates { get; set; } = new();

        /// <summary>
        /// Timestamp of the last update.
        /// </summary>
        public string Last_update { get; set; } = string.Empty;
    }
}
