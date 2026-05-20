using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Michitai.Multiplayer.Rooms.Actions
{
    /// <summary>
    /// Request data for completing an action in a room.
    /// Serializes completion status and response data for API transmission using Unity's JsonUtility.
    /// </summary>
    [System.Serializable]
    public class ActionCompleteRequest
    {
        /// <summary>
        /// The status of the action completion (default: Completed).
        /// </summary>
        public string status = ERoomCompleteActionStatus.Completed.ToString().ToLower();

        /// <summary>
        /// Serialized response data (Unity mode).
        /// </summary>
        public string response_data_json;

        /// <summary>
        /// Initializes a new ActionCompleteRequest.
        /// </summary>
        /// <param name="status">The status of the action completion.</param>
        /// <param name="responseDataJson">Serialized response data.</param>
        public ActionCompleteRequest(string status, string responseDataJson)
        {
            this.status = status;
            this.response_data_json = responseDataJson;
        }
    }
}
