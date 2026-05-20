using Michitai.Multiplayer.Errors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Michitai.Multiplayer.Rooms
{
    /// <summary>
    /// Response containing a list of available game rooms.
    /// </summary>
    /// <typeparam name="T">The type to deserialize room rules into.</typeparam>
    public class RoomListResponse<T> : ApiResponse<ERoomListError> where T : class, new()
    {
        /// <summary>
        /// List of available game rooms with their details.
        /// </summary>
        public List<RoomShort<T>> Rooms { get; set; } = new();
    }
}
