using Michitai.Multiplayer.Errors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Michitai.Multiplayer.Matchmaking.Requests
{
    /// <summary>
    /// Response containing the status of a specific join request.
    /// </summary>
    [System.Serializable]
    public class MatchmakingRequestStatusResponse : ApiResponse<EMatchmakingStatusError>
    {
        /// <summary>
        /// The join request information including status and details.
        /// </summary>
        public MatchmakingRequestInfo request;
    }
}
