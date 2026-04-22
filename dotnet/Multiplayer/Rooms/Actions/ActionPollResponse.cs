using Michitai.Multiplayer.Errors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Michitai.Multiplayer.Rooms.Actions
{
    public class ActionPollResponse<T> : ApiResponse<ERoomActionsPollError> where T : class, new()
    {
        public List<ActionInfo<T>> Actions { get; set; } = new();
    }
}
