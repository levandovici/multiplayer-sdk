using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Michitai.Multiplayer.Rooms.Actions
{
    /// <summary>
    /// Information about a completed action in the room.
    /// Contains action details, status, and response data.
    /// Uses Unity's JsonUtility for deserialization of response data.
    /// </summary>
    [System.Serializable]
    public class ActionInfo
    {
        [SerializeField]
        private string status;
        [SerializeField]
        private string response_data_json;   // Unity mode

        /// <summary>
        /// The unique ID of the action.
        /// </summary>
        public string action_id;

        /// <summary>
        /// The type of action that was performed.
        /// </summary>
        public string action_type;

        /// <summary>
        /// Whether the action was sent by the host.
        /// </summary>
        public bool is_host;

        /// <summary>
        /// The parsed status of the action.
        /// </summary>
        public ERoomActionStatus Status
        {
            get
            {
                switch (status)
                {
                    case "pending":
                        return ERoomActionStatus.Pending;
                    case "processing":
                        return ERoomActionStatus.Processing;
                    case "completed":
                        return ERoomActionStatus.Completed;
                    case "failed":
                        return ERoomActionStatus.Failed;
                    case "read":
                        return ERoomActionStatus.Read;
                    default:
                        throw new System.ArgumentException($"Unknown action status: {status}");
                }
            }
        }
    }
}
