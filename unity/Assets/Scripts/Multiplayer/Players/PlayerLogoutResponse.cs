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
    public class PlayerLogoutResponse : ApiResponse<EPlayerLogoutError>
    {
        [SerializeField]
        private string last_logout;



        public string message;



        public DateTimeOffset? LastLogout
        {
            get
            {
                return ParseUtc(last_logout);
            }
        }
    }
}
