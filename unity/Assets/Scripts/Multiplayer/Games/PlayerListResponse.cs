using Michitai.Multiplayer.Errors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Michitai.Multiplayer.Games
{
    /// <summary>
    /// Response containing a list of all players in the game in Unity.
    /// </summary>
    [System.Serializable]
    public class PlayerListResponse : ApiResponse<EPlayerListError>
    {
        /// <summary>
        /// The total number of players in the game.
        /// </summary>
        public int count;

        /// <summary>
        /// List of players with their basic information.
        /// </summary>
        public List<PlayerShort> players = new();
    }
}
