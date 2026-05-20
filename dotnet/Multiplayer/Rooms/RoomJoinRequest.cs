using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Michitai.Multiplayer.Rooms
{
    /// <summary>
    /// Request data for joining a game room.
    /// </summary>
    /// <typeparam name="T">The type of optional player data to include.</typeparam>
    public class RoomJoinRequest<T> where T : class, new()
    {
        [JsonInclude]
        private string? Password { get; set; }
        [JsonInclude]
        private T? Player_data { get; set; }

        /// <summary>
        /// Initializes a new RoomJoinRequest.
        /// </summary>
        /// <param name="password">Optional password for the room.</param>
        /// <param name="playerData">Optional player data to include.</param>
        public RoomJoinRequest(string? password = null, T? playerData = null)
        {
            this.Password = password;
            this.Player_data = playerData;
        }
    }
}
