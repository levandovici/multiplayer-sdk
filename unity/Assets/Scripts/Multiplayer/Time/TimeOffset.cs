using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Michitai.Multiplayer.Time
{
    [System.Serializable]
    public class TimeOffset
    {
        [SerializeField]
        private string original_utc;



        public int offset_hours;
        public string offset_string;
        public long original_timestamp;



        public DateTimeOffset? OriginalUtc
        {
            get
            {
                return Time.ParseUtc(original_utc);
            }
        }
    }
}
