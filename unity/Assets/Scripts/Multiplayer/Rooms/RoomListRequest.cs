using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Michitai.Multiplayer.Rooms
{
    /// <summary>
    /// Request data for listing game rooms with optional filtering in Unity.
    /// </summary>
    public class RoomListRequest
    {
        /// <summary>
        /// Search term to filter rooms by name.
        /// </summary>
        public string search = "";

        /// <summary>
        /// Maximum number of rooms to return.
        /// </summary>
        public int limit = 20;
    }
}
