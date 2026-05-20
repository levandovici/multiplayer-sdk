using System;
using UnityEngine;

namespace Michitai.Multiplayer.Players
{
    /// <summary>
    /// Internal request data for unbanning a player from the game in Unity.
    /// </summary>
    [Serializable]
    internal class PlayerUnbanRequest
    {
        /// <summary>
        /// The ID of the player to unban.
        /// </summary>
        public int player_id;

        /// <summary>
        /// Initializes a new PlayerUnbanRequest.
        /// </summary>
        /// <param name="playerId">The ID of the player to unban.</param>
        public PlayerUnbanRequest(int playerId)
        {
            this.player_id = playerId;
        }
    }
}
