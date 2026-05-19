using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Michitai.Multiplayer.Matchmaking
{
    public class MatchmakingPasswordUpdateRequest
    {
        [JsonInclude]
        public string? Password { get; set; }

        public MatchmakingPasswordUpdateRequest(string? password = null)
        {
            this.Password = password;
        }
    }
}
