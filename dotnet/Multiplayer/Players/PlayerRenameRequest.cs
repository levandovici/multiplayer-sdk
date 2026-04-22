using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Michitai.Multiplayer.Players
{
    internal class PlayerRenameRequest
    {
        [JsonInclude]
        internal required string New_name { get; set; }



        [SetsRequiredMembers]
        public PlayerRenameRequest(string newName)
        {
            this.New_name = newName;
        }
    }
}
