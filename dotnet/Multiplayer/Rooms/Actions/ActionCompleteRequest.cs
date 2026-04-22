using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Michitai.Multiplayer.Rooms.Actions
{
    public class ActionCompleteRequest<T> where T : class, new()
    {
        [JsonInclude]
        private string Status { get; set; } = ERoomCompleteActionStatus.Completed.ToString().ToLower();
        [JsonInclude]
        private T? Response_data { get; set; }



        public ActionCompleteRequest(ERoomCompleteActionStatus status, T? responseData)
        {
            this.Status = status.ToString().ToLower();
            this.Response_data = responseData;
        }
    }
}
