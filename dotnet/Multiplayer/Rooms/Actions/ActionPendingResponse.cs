using Michitai.Multiplayer.Errors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Michitai.Multiplayer.Rooms.Actions
{
    /// <summary>
    /// Response containing pending actions that need to be completed by the host.
    /// Only the host can retrieve pending actions.
    /// </summary>
    /// <typeparam name="T">The type to deserialize action data into.</typeparam>
    public class ActionPendingResponse<T> : ApiResponse<ERoomActionsPendingError> where T : class, new()
    {
        /// <summary>
        /// List of pending actions awaiting completion.
        /// </summary>
        public List<PendingAction<T>> Actions { get; set; } = new();
    }
}
