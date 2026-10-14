using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static Michitai.Multiplayer.Time.Time;

namespace Michitai.Multiplayer.Friends
{
    /// <summary>
    /// A pending lobby/room invitation addressed to the local player.
    /// </summary>
    [System.Serializable]
    public class FriendInvite
    {
        [SerializeField]
        private string created_at;

        /// <summary>
        /// The invite's unique ID — pass to AcceptInviteAsync/DeclineInviteAsync.
        /// </summary>
        public string invite_id;

        /// <summary>
        /// What the invite joins: "matchmaking" (a lobby) or "room" (a running game).
        /// </summary>
        public string kind;

        /// <summary>
        /// Display name of the lobby or room.
        /// </summary>
        public string name;

        /// <summary>
        /// The friend who sent the invite.
        /// </summary>
        public FriendInfo from_player;

        /// <summary>
        /// Timestamp when the invite was created.
        /// </summary>
        public DateTimeOffset? CreatedAt
        {
            get
            {
                return ParseUtc(created_at);
            }
        }
    }
}
