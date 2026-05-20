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
    /// Response returned when a player is successfully logged out.
    /// </summary>
    [System.Serializable]
    public class PlayerLogoutResponse : ApiResponse<EPlayerLogoutError>
    {
        [SerializeField]
        private string last_logout;

        /// <summary>
        /// Confirmation message for the logout.
        /// </summary>
        public string message;

        /// <summary>
        /// Timestamp when the player was logged out (parsed from string).
        /// </summary>
        public DateTimeOffset? LastLogout
        {
            get
            {
                return ParseUtc(last_logout);
            }
        }
    }
}
