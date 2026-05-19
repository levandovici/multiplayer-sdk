using Michitai.Multiplayer.Errors;
using System;
using UnityEngine;

namespace Michitai.Multiplayer.Rooms
{
    [Serializable]
    public class RoomKickResponse : ApiResponse<ERoomKickError>
    {
        public string message;
        public int kicked_player_id;
    }
}
