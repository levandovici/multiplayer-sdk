using Michitai.Multiplayer.Errors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Michitai.Multiplayer.Rooms.Actions
{
    public class ActionCompleteResponse : ApiResponse<ERoomActionsCompleteError>
    {
        public string Message { get; set; } = string.Empty;
    }
}
