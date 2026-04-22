using Michitai.Multiplayer.Errors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Michitai.Multiplayer.Games
{
    public class PlayerListResponse : ApiResponse<EPlayerListError>
    {
        public int Count { get; set; }
        public List<PlayerShort> Players { get; set; } = new();
    }
}
