using Michitai.Multiplayer.Errors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Michitai.Multiplayer
{
    /// <summary>
    /// Standard success response for API operations that don't return specific data.
    /// Includes a message and timestamp for confirmation.
    /// </summary>
    public class SuccessResponse : ApiResponse<ECommonError>
    {
        /// <summary>
        /// Success message describing the operation result.
        /// </summary>
        public string Message { get; set; } = string.Empty;

        /// <summary>
        /// Timestamp when the operation was completed.
        /// </summary>
        public DateTimeOffset Updated_at { get; set; }
    }
}
