using Michitai.Multiplayer.Errors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Michitai.Multiplayer.Matchmaking
{
    public class MatchmakingListResponse<T> : ApiResponse<EMatchmakingListError> where T : class, new()
    {
        public List<MatchmakingLobby<T>> Lobbies { get; set; } = new();
    }
}
