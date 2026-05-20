using Michitai.Multiplayer.Errors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Michitai.Multiplayer.Rooms.Actions
{
    /// <summary>
    /// Response containing completed actions that were targeted to the polling player.
    /// </summary>
    [System.Serializable]
    public class ActionPollResponse : ApiResponse<ERoomActionsPollError>
    {
        /// <summary>
        /// List of completed actions with their details.
        /// </summary>
        public List<ActionInfo> actions = new();
    }
}
