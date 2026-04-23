using Michitai.Multiplayer.Errors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Michitai.Multiplayer.Players
{
    [System.Serializable]
    public class PlayerAuthResponse<T> : ApiResponse<EPlayerLoginError> where T : class, new()
    {
        public PlayerInfo<T> player;
    }
}
