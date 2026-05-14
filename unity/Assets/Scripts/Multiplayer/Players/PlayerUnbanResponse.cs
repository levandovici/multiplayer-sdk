using Michitai.Multiplayer.Errors;
using System;
using UnityEngine;

namespace Michitai.Multiplayer.Players
{
    [Serializable]
    public class PlayerUnbanResponse : ApiResponse<ECommonError>
    {
        public string message;
        public int player_id;
    }
}
