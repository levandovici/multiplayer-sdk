using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Michitai.Multiplayer.Friends
{
    /// <summary>
    /// Internal request body for accepting or declining a lobby/room invite.
    /// </summary>
    [System.Serializable]
    internal class FriendInviteRespondRequest
    {
        /// <summary>
        /// The invite to respond to.
        /// </summary>
        public string invite_id;

        /// <summary>
        /// Serialized JSON string of player data to join with (accept only).
        /// </summary>
        public string player_data_json;

        /// <summary>
        /// Initializes a new FriendInviteRespondRequest.
        /// </summary>
        /// <param name="inviteId">The invite ID.</param>
        /// <param name="playerDataJson">Serialized player data for the join.</param>
        public FriendInviteRespondRequest(string inviteId, string playerDataJson = null)
        {
            this.invite_id = inviteId;
            this.player_data_json = playerDataJson;
        }
    }
}
