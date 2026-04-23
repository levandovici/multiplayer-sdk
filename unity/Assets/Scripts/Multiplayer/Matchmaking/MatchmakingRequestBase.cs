using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static Michitai.Multiplayer.Time.Time;

namespace Michitai.Multiplayer.Matchmaking
{
    [System.Serializable]
    public class MatchmakingRequestBase
    {
        [SerializeField]
        private string requested_at;
        [SerializeField]
        private string responded_at;



        public string request_id;
        public string matchmaking_id;
        public string status;



        public DateTimeOffset? RequestedAt
        {
            get
            {
                return ParseUtc(requested_at);
            }
        }

        public DateTimeOffset? RespondedAt
        {
            get
            {
                return ParseUtc(responded_at);
            }
        }
    }
}
