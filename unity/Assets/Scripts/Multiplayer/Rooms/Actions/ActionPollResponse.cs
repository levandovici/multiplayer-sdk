using Michitai.Multiplayer.Errors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Michitai.Multiplayer.Rooms.Actions
{
    [System.Serializable]
    public class ActionPollResponse : ApiResponse<ERoomActionsPollError>
    {
        public List<ActionInfo> actions = new();
    }
}
