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
    /// <summary>
    /// Standard success response for API operations that don't return specific data in Unity.
    /// Includes a message and timestamp for confirmation with Unity-specific time parsing.
    /// </summary>
    [System.Serializable]
    public class SuccessResponse : ApiResponse<ECommonError>
    {
        [SerializeField]
        private string updated_at;

        /// <summary>
        /// Success message describing the operation result.
        /// </summary>
        public string message;

        /// <summary>
        /// Parsed timestamp when the operation was completed.
        /// Uses Unity-specific UTC parsing.
        /// </summary>
        public DateTimeOffset? UpdatedAt
        {
            get
            {
                return ParseUtc(updated_at);
            }
        }
    }
}
