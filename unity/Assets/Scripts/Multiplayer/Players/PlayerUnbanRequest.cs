using System;
using UnityEngine;

namespace Michitai.Multiplayer.Players
{
    [Serializable]
    internal class PlayerUnbanRequest
    {
        public int player_id;

        public PlayerUnbanRequest(int playerId)
        {
            this.player_id = playerId;
        }
    }
}
