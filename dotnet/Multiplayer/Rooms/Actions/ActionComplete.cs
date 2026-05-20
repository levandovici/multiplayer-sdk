using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Michitai.Multiplayer.Rooms.Actions
{
    /// <summary>
    /// Request parameters for completing an action in a room.
    /// Only the host can complete actions.
    /// </summary>
    /// <typeparam name="T">The type of response data.</typeparam>
    public class ActionComplete<T> where T : class, new()
    {
        /// <summary>
        /// The status of the action completion.
        /// </summary>
        public ERoomCompleteActionStatus Status { get; private set; }

        /// <summary>
        /// The response data to return with the completion.
        /// </summary>
        public T? Response_data { get; private set; }

        /// <summary>
        /// Initializes a new ActionComplete request.
        /// </summary>
        /// <param name="status">The status of the action completion.</param>
        /// <param name="responseData">The response data to return with the completion.</param>
        public ActionComplete(ERoomCompleteActionStatus status, T? responseData)
        {
            Status = status;
            Response_data = responseData;
        }
    }
}
