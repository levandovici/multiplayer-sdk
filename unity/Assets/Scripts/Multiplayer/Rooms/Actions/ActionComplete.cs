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
        private ERoomCompleteActionStatus _status;
        private T _response_data;

        /// <summary>
        /// The status of the action completion.
        /// </summary>
        public ERoomCompleteActionStatus Status
        {
            get
            {
                return _status;
            }

            private set
            {
                _status = value;
            }
        }

        /// <summary>
        /// The response data to return with the completion.
        /// </summary>
        public T ResponseData
        {
            get
            {
                return _response_data;
            }

            private set
            {
                _response_data = value;
            }
        }

        /// <summary>
        /// Initializes a new ActionComplete request.
        /// </summary>
        /// <param name="status">The status of the action completion.</param>
        /// <param name="response_data">The response data to return with the completion.</param>
        public ActionComplete(ERoomCompleteActionStatus status, T response_data)
        {
            Status = status;
            ResponseData = response_data;
        }
    }
}
