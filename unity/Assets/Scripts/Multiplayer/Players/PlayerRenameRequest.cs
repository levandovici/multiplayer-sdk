using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Michitai.Multiplayer.Players
{
    /// <summary>
    /// Internal request data for renaming a player in Unity.
    /// </summary>
    [System.Serializable]
    internal class PlayerRenameRequest
    {
        /// <summary>
        /// The new name for the player.
        /// </summary>
        public string new_name;

        /// <summary>
        /// Initializes a new PlayerRenameRequest.
        /// </summary>
        /// <param name="newName">The new name for the player.</param>
        public PlayerRenameRequest(string newName)
        {
            this.new_name = newName;
        }
    }
}
