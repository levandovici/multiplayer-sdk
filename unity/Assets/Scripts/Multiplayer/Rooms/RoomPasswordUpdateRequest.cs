using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Michitai.Multiplayer.Rooms
{
    /// <summary>
    /// Internal request data for updating a game room password in Unity.
    /// </summary>
    [System.Serializable]
    internal class RoomPasswordUpdateRequest
    {
        /// <summary>
        /// The new password for the room.
        /// </summary>
        public string password;

        /// <summary>
        /// Initializes a new RoomPasswordUpdateRequest.
        /// </summary>
        /// <param name="password">The new password for the room.</param>
        public RoomPasswordUpdateRequest(string password)
        {
            this.password = password;
        }
    }
}
