using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Michitai.Multiplayer.Players
{
    /// <summary>
    /// Internal request data for renaming a player.
    /// </summary>
    internal class PlayerRenameRequest
    {
        [JsonInclude]
        internal required string New_name { get; set; }

        /// <summary>
        /// Initializes a new PlayerRenameRequest.
        /// </summary>
        /// <param name="newName">The new name for the player.</param>
        [SetsRequiredMembers]
        public PlayerRenameRequest(string newName)
        {
            this.New_name = newName;
        }
    }
}
