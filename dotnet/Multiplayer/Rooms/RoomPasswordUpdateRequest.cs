using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Michitai.Multiplayer.Rooms
{
    /// <summary>
    /// Request data for updating a game room password.
    /// </summary>
    public class RoomPasswordUpdateRequest
    {
        [JsonInclude]
        public string? Password { get; set; }

        /// <summary>
        /// Initializes a new RoomPasswordUpdateRequest.
        /// </summary>
        /// <param name="password">The new password for the room, or null to remove password.</param>
        public RoomPasswordUpdateRequest(string? password = null)
        {
            this.Password = password;
        }
    }
}
