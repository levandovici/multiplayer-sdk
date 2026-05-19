using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Michitai.Multiplayer.Rooms
{
    [System.Serializable]
    internal class RoomPasswordUpdateRequest
    {
        public string password;

        public RoomPasswordUpdateRequest(string password)
        {
            this.password = password;
        }
    }
}
