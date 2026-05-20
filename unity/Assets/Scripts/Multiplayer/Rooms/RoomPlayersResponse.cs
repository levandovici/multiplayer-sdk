using Michitai.Multiplayer.Errors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Michitai.Multiplayer.Rooms
{
    /// <summary>
    /// Response containing the list of players in a game room.
    /// </summary>
    /// <typeparam name="T">The type to deserialize player data into.</typeparam>
    [System.Serializable]
    public class RoomPlayersResponse<T> : ApiResponse<ERoomPlayersError> where T : class, new()
    {
        /// <summary>
        /// List of players currently in the game room.
        /// </summary>
        public List<RoomPlayer<T>> players = new();

        /// <summary>
        /// Timestamp of when the player list was last updated.
        /// </summary>
        public string last_updated;
    }
}
