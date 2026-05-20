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
    /// <typeparam name="T">The type to deserialize action data into.</typeparam>
    public class ActionPollResponse<T> : ApiResponse<ERoomActionsPollError> where T : class, new()
    {
        /// <summary>
        /// List of completed actions with their details.
        /// </summary>
        public List<ActionInfo<T>> Actions { get; set; } = new();
    }
}
