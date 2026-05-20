using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Michitai.Multiplayer.Rooms
{
    /// <summary>
    /// Internal request data for joining a game room in Unity.
    /// Uses serialized JSON string for player data.
    /// </summary>
    [System.Serializable]
    internal class RoomJoinRequest
    {
        /// <summary>
        /// The password for the room.
        /// </summary>
        public string password;

        /// <summary>
        /// Serialized JSON string of player data (Unity mode).
        /// </summary>
        public string player_data_json;

        /// <summary>
        /// Initializes a new RoomJoinRequest.
        /// </summary>
        /// <param name="password">The password for the room.</param>
        /// <param name="playerData">Serialized JSON string of player data.</param>
        public RoomJoinRequest(string password, string playerData)
        {
            this.password = password;
            this.player_data_json = playerData;
        }
    }
}
