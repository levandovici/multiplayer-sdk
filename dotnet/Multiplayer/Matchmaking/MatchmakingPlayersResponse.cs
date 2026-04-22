using Michitai.Multiplayer.Errors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Michitai.Multiplayer.Matchmaking
{
    public class MatchmakingPlayersResponse<T> : ApiResponse<EMatchmakingPlayersError> where T : class, new()
    {
        public List<MatchmakingPlayer<T>> Players { get; set; } = new();
    }
}
