using System;
using UnityEngine;

namespace Michitai.Multiplayer.Players
{
    /// <summary>
    /// Internal request data for banning a player from the game in Unity.
    /// </summary>
    [Serializable]
    internal class PlayerBanRequest
    {
        /// <summary>
        /// The ID of the player to ban.
        /// </summary>
        public int player_id;

        /// <summary>
        /// The duration of the ban.
        /// </summary>
        public string ban_duration;

        /// <summary>
        /// The reason for the ban.
        /// </summary>
        public string ban_reason;

        /// <summary>
        /// Initializes a new PlayerBanRequest.
        /// </summary>
        /// <param name="playerId">The ID of the player to ban.</param>
        /// <param name="banDuration">The duration of the ban.</param>
        /// <param name="banReason">Optional reason for the ban.</param>
        public PlayerBanRequest(int playerId, EBanTime banDuration, string banReason = null)
        {
            this.player_id = playerId;
            this.ban_duration = banDuration.ToString().ToLower();
            this.ban_reason = banReason;
        }
    }
}
