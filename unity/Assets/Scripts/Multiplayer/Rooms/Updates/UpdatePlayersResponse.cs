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
    [System.Serializable]
    public class UpdatePlayersResponse : ApiResponse<ERoomUpdatesError>
    {
        /// <summary>
        /// The number of updates that were sent.
        /// </summary>
        public int updates_sent;

        /// <summary>
        /// List of IDs for the sent updates.
        /// </summary>
        public List<string> update_ids = new();

        /// <summary>
        /// List of target player IDs for the updates.
        /// </summary>
        public List<int> target_players_ids = new();
    }
}
