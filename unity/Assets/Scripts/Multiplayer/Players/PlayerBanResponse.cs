using Michitai.Multiplayer.Errors;
using System;
using UnityEngine;

namespace Michitai.Multiplayer.Players
{
    [Serializable]
    public class PlayerBanResponse : ApiResponse<ECommonError>
    {
        public string message;
        public string ban_id;
        public int player_id;
        public string ban_duration;
        public string banned_until;
    }
}
