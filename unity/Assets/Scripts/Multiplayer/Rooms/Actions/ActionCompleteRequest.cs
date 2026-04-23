using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Michitai.Multiplayer.Rooms.Actions
{
    [System.Serializable]
    public class ActionCompleteRequest
    {
        public string status = ERoomCompleteActionStatus.Completed.ToString().ToLower();
        public string response_data_json;



        public ActionCompleteRequest(string status, string responseDataJson)
        {
            this.status = status;
            this.response_data_json = responseDataJson;
        }
    }
}
