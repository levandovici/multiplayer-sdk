using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Michitai.Multiplayer.Rooms
{
    /// <summary>
    /// Request data for listing game rooms with optional filtering.
    /// </summary>
    public class RoomListRequest
    {
        /// <summary>
        /// Optional search term to filter rooms by name.
        /// </summary>
        public string? Search { get; set; }

        /// <summary>
        /// Optional maximum number of rooms to return.
        /// </summary>
        public int? Limit { get; set; }
    }
}
