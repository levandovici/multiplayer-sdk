using Michitai.Multiplayer.Errors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Michitai.Multiplayer.Games
{
    [System.Serializable]
    public class PlayerListResponse : ApiResponse<EPlayerListError>
    {
        public int count;
        public List<PlayerShort> players = new();
    }
}
