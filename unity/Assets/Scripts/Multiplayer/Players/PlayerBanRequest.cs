using System;
using UnityEngine;

namespace Michitai.Multiplayer.Players
{
    [Serializable]
    internal class PlayerBanRequest
    {
        public int player_id;
        public string ban_duration;
        public string ban_reason;

        public PlayerBanRequest(int playerId, EBanTime banDuration, string banReason = null)
        {
            this.player_id = playerId;
            this.ban_duration = banDuration.ToString().ToLower();
            this.ban_reason = banReason;
        }
    }
}
