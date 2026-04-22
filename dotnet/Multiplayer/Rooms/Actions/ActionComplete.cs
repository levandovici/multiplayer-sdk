using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Michitai.Multiplayer.Rooms.Actions
{
    public class ActionComplete<T> where T : class, new()
    {
        public ERoomCompleteActionStatus Status { get; private set; }

        public T? Response_data { get; private set; }



        public ActionComplete(ERoomCompleteActionStatus status, T? responseData)
        {
            Status = status;
            Response_data = responseData;
        }
    }
}
