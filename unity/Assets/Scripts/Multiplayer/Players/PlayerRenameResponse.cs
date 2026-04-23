using Michitai.Multiplayer.Errors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Michitai.Multiplayer.Players
{
    [System.Serializable]
    public class PlayerRenameResponse : ApiResponse<EPlayerRenameError>
    {
        public string message;
        public string new_name;
        public int player_id;
    }
}
