using Michitai.Multiplayer.Errors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Michitai.Multiplayer.Rooms.Actions
{
    [System.Serializable]
    public class ActionPendingResponse<T> : ApiResponse<ERoomActionsPendingError> where T : class, new()
    {
        public List<PendingAction<T>> actions = new();
    }
}
