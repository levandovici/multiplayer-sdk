using Michitai.Multiplayer.Errors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Michitai.Multiplayer.Rooms.Updates
{
    /// <summary>
    /// Response returned when updates are successfully sent to target players.
    /// </summary>
    public class UpdatePlayersResponse : ApiResponse<ERoomUpdatesError>
    {
        /// <summary>
        /// The number of updates that were sent.
        /// </summary>
        public int Updates_sent { get; set; }

        /// <summary>
        /// List of IDs for the sent updates.
        /// </summary>
        public List<string> Update_ids { get; set; } = new();

        /// <summary>
        /// List of target player IDs for the updates.
        /// </summary>
        public List<int> Target_players_ids { get; set; } = new();
    }
}
