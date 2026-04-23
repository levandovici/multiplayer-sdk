using Michitai.Multiplayer.Errors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static Michitai.Multiplayer.Time.Time;

namespace Michitai.Multiplayer.Players
{
    [System.Serializable]
    public class PlayerHeartbeatResponse : ApiResponse<EPlayerHeartbeatError>
    {
        [SerializeField]
        private string last_heartbeat;



        public string message;



        public DateTimeOffset? LastHeartbeat
        {
            get
            {
                return ParseUtc(last_heartbeat);
            }
        }
    }
}
