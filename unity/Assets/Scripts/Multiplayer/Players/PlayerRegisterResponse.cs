using Michitai.Multiplayer.Errors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Michitai.Multiplayer.Players
{
    [System.Serializable]
    public class PlayerRegisterResponse : ApiResponse<EPlayerRegisterError>
    {
        public int player_id;
        public string private_key;
        public string player_name;
        public int game_id;
    }
}
