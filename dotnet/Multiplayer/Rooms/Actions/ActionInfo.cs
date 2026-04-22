using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Michitai.Multiplayer.Rooms.Actions
{
    public class ActionInfo<T> where T : class, new()
    {
        [JsonInclude]
        private string Status { get; set; } = string.Empty;



        public string Action_id { get; set; } = string.Empty;
        public string Action_type { get; set; } = string.Empty;
        public bool Is_host { get; set; }
        public T? Response_data { get; set; }



        public ERoomActionStatus ActionStatus
        {
            get
            {
                switch (Status)
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
                        throw new ArgumentException($"Unknown action status: {Status}");
                }
            }
        }
    }
}
