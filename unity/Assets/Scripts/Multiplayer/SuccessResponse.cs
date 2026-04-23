using Michitai.Multiplayer.Errors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static Michitai.Multiplayer.Time.Time;

namespace Michitai.Multiplayer
{
    [System.Serializable]
    public class SuccessResponse : ApiResponse<ECommonError>
    {
        [SerializeField]
        private string updated_at;



        public string message;



        public DateTimeOffset? UpdatedAt
        {
            get
            {
                return ParseUtc(updated_at);
            }
        }
    }
}
