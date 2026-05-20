using Michitai.Multiplayer.Errors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Michitai.Multiplayer.Players
{
    /// <summary>
    /// Response returned when a player is successfully authenticated.
    /// Contains the player's full information including typed player data.
    /// </summary>
    /// <typeparam name="T">The type to deserialize player data into.</typeparam>
    [System.Serializable]
    public class PlayerAuthResponse<T> : ApiResponse<EPlayerLoginError> where T : class, new()
    {
        /// <summary>
        /// The authenticated player's information including ID, name, and custom data.
        /// </summary>
        public PlayerInfo<T> player;
    }
}
