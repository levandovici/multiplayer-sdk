using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Michitai.Multiplayer.Matchmaking
{
    [System.Serializable]
    internal class MatchmakingPasswordUpdateRequest
    {
        public string password;

        public MatchmakingPasswordUpdateRequest(string password)
        {
            this.password = password;
        }
    }
}
