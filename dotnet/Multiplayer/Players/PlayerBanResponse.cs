using Michitai.Multiplayer.Errors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Michitai.Multiplayer.Players
{
    public class PlayerBanResponse : ApiResponse<ECommonError>
    {
        public string Message { get; set; } = string.Empty;
        public string Ban_id { get; set; } = string.Empty;
        public int Player_id { get; set; }
        public string Ban_duration { get; set; } = string.Empty;
        public string Banned_until { get; set; } = string.Empty;
    }
}
