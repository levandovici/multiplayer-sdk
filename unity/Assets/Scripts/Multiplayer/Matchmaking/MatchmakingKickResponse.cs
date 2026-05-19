using Michitai.Multiplayer.Errors;
using System;
using UnityEngine;

namespace Michitai.Multiplayer.Matchmaking
{
    [Serializable]
    public class MatchmakingKickResponse : ApiResponse<EMatchmakingKickError>
    {
        public string message;
        public int kicked_player_id;
    }
}
