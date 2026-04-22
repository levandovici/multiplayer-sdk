using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Michitai.Multiplayer.Players
{
    internal class PlayerRegisterRequest<T> where T : class, new()
    {
        [JsonInclude]
        internal required string Player_name { get; set; }
        [JsonInclude]
        private T? Player_data { get; set; }



        [SetsRequiredMembers]
        public PlayerRegisterRequest(string playerName, T? playerData = null)
        {
            this.Player_name = playerName;
            this.Player_data = playerData;
        }
    }
}
