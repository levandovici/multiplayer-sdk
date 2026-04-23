using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Michitai.Multiplayer.Rooms.Actions
{
    public class ActionComplete<T> where T : class, new()
    {
        private ERoomCompleteActionStatus _status;
        private T _response_data;



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



        public ActionComplete(ERoomCompleteActionStatus status, T response_data)
        {
            Status = status;
            ResponseData = response_data;
        }
    }
}
