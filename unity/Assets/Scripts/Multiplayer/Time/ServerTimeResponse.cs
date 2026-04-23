using Michitai.Multiplayer.Errors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Michitai.Multiplayer.Time
{
    [System.Serializable]
    public class ServerTimeResponse : ApiResponse<ETimeError>
    {
        [SerializeField]
        private string utc;



        public long timestamp;
        public string readable;



        public DateTimeOffset? Utc
        {
            get
            {
                return Time.ParseUtc(utc);
            }
        }
    }
}
