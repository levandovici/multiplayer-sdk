using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Michitai.Multiplayer.Friends
{
    /// <summary>
    /// Internal request body targeting another player by id or tag.
    /// </summary>
    [System.Serializable]
    internal class FriendTargetRequest
    {
        /// <summary>
        /// The target player's ID (0 = unused).
        /// </summary>
        public int player_id;

        /// <summary>
        /// The target player's friend tag in display or raw form.
        /// </summary>
        public string player_tag;

        /// <summary>
        /// Initializes a new FriendTargetRequest.
        /// </summary>
        /// <param name="playerId">The target player's ID.</param>
        /// <param name="playerTag">The target player's friend tag.</param>
        public FriendTargetRequest(int playerId = 0, string playerTag = null)
        {
            this.player_id = playerId;
            this.player_tag = playerTag;
        }
    }
}
