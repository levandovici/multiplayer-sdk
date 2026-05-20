using Michitai.Multiplayer.Errors;
using System;
using UnityEngine;

namespace Michitai.Multiplayer.Matchmaking
{
    /// <summary>
    /// Response returned when a player is successfully kicked from a matchmaking lobby.
    /// </summary>
    [Serializable]
    public class MatchmakingKickResponse : ApiResponse<EMatchmakingKickError>
    {
        /// <summary>
        /// Confirmation message for the kick operation.
        /// </summary>
        public string message;

        /// <summary>
        /// The ID of the player who was kicked.
        /// </summary>
        public int kicked_player_id;
    }
}
