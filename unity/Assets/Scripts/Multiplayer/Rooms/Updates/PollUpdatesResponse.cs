using Michitai.Multiplayer.Errors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Michitai.Multiplayer.Rooms.Updates
{
    [System.Serializable]
    public class PollUpdatesResponse : ApiResponse<ERoomUpdatesPollError>
    {
        public List<PlayerUpdate> updates = new();
        public string last_update;
    }
}
