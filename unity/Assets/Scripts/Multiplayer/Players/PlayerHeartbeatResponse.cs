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
    /// <summary>
    /// Response returned when a player heartbeat is successfully sent.
    /// Used to maintain the player's online status.
    /// </summary>
    [System.Serializable]
    public class PlayerHeartbeatResponse : ApiResponse<EPlayerHeartbeatError>
    {
        [SerializeField]
        private string last_heartbeat;

        /// <summary>
        /// Confirmation message for the heartbeat.
        /// </summary>
        public string message;

        /// <summary>
        /// Timestamp of the last heartbeat received (parsed from string).
        /// </summary>
        public DateTimeOffset? LastHeartbeat
        {
            get
            {
                return ParseUtc(last_heartbeat);
            }
        }
    }
}
