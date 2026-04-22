using Michitai.Multiplayer.Errors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Michitai.Multiplayer.Players
{
    public class PlayerRenameResponse : ApiResponse<EPlayerRenameError>
    {
        public string Message { get; set; } = string.Empty;
        public string New_name { get; set; } = string.Empty;
        public int Player_id { get; set; }
    }
}
