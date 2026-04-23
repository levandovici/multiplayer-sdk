using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Michitai.Multiplayer.Rooms.Actions
{
    [System.Serializable]
    public class ActionInfo
    {
        [SerializeField]
        private string status;
        [SerializeField]
        private string response_data_json;   // Unity mode



        public string action_id;
        public string action_type;
        public bool is_host;



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
