using Michitai.Multiplayer.Errors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Michitai.Multiplayer
{
    public class SuccessResponse : ApiResponse<ECommonError>
    {
        public string Message { get; set; } = string.Empty;
        public DateTimeOffset Updated_at { get; set; }
    }
}
