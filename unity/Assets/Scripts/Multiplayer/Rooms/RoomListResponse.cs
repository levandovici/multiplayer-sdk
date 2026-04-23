using Michitai.Multiplayer.Errors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Michitai.Multiplayer.Rooms
{
    [System.Serializable]
    public class RoomListResponse<T> : ApiResponse<ERoomListError> where T : class, new()
    {
        public List<RoomShort<T>> rooms = new();
    }
}
