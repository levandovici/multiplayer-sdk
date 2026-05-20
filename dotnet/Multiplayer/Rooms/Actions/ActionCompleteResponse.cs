using Michitai.Multiplayer.Errors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Michitai.Multiplayer.Rooms.Actions
{
    /// <summary>
    /// Response returned when an action is successfully marked as complete.
    /// Only the host can complete actions.
    /// </summary>
    public class ActionCompleteResponse : ApiResponse<ERoomActionsCompleteError>
    {
        /// <summary>
        /// Confirmation message for the action completion.
        /// </summary>
        public string Message { get; set; } = string.Empty;
    }
}
