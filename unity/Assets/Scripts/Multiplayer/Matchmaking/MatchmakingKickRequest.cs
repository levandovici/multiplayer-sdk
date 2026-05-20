using System;
using UnityEngine;

namespace Michitai.Multiplayer.Matchmaking
{
    /// <summary>
    /// Request data for kicking a player from a matchmaking lobby in Unity.
    /// </summary>
    [Serializable]
    public class MatchmakingKickRequest
    {
        /// <summary>
        /// The ID of the player to kick.
        /// </summary>
        public int player_id;
    }
}
