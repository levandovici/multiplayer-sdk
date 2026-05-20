using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Michitai.Multiplayer.Rooms.Actions
{
    /// <summary>
    /// Request data for completing an action in a room.
    /// Serializes completion status and response data for API transmission.
    /// </summary>
    /// <typeparam name="T">The type of response data.</typeparam>
    public class ActionCompleteRequest<T> where T : class, new()
    {
        [JsonInclude]
        private string Status { get; set; } = ERoomCompleteActionStatus.Completed.ToString().ToLower();
        [JsonInclude]
        private T? Response_data { get; set; }

        /// <summary>
        /// Initializes a new ActionCompleteRequest.
        /// </summary>
        /// <param name="status">The status of the action completion.</param>
        /// <param name="responseData">The response data to return with the completion.</param>
        public ActionCompleteRequest(ERoomCompleteActionStatus status, T? responseData)
        {
            this.Status = status.ToString().ToLower();
            this.Response_data = responseData;
        }
    }
}
