using Michitai.Multiplayer.Errors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Michitai.Multiplayer.Players
{
    public class PlayerRegisterResponse : ApiResponse<EPlayerRegisterError>
    {
        public int Player_id { get; set; }
        public string Private_key { get; set; } = string.Empty;
        public string Player_name { get; set; } = string.Empty;
        public int Game_id { get; set; }
    }
}
