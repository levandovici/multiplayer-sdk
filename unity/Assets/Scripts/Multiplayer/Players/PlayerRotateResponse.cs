using Michitai.Multiplayer.Errors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Michitai.Multiplayer.Players
{
    /// <summary>
    /// Response returned when a player's private key is successfully rotated.
    /// The previous token is invalidated; store the new key returned here.
    /// </summary>
    [System.Serializable]
    public class PlayerRotateResponse : ApiResponse<EPlayerLoginError>
    {
        /// <summary>
        /// The new private key token. Shown only once - persist it client-side.
        /// </summary>
        public string private_key;

        /// <summary>
        /// The ID of the player whose key was rotated.
        /// </summary>
        public int player_id;
    }
}
