using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Michitai.Multiplayer.Rooms.Updates
{
    /// <summary>
    /// Internal request data for polling player updates in Unity.
    /// </summary>
    [System.Serializable]
    internal class PollUpdatesRequest
    {
        /// <summary>
        /// The source players to poll updates from (all, host, others, specific).
        /// </summary>
        public string from_players = ERoomTargetPlayers.Host.ToString().ToLower();

        /// <summary>
        /// Specific player IDs if from_players is specific.
        /// </summary>
        public int[] from_players_ids;

        /// <summary>
        /// The last update ID to poll from.
        /// </summary>
        public string last_update;

        /// <summary>
        /// Initializes a new PollUpdatesRequest.
        /// </summary>
        /// <param name="fromPlayers">The source players to poll updates from.</param>
        /// <param name="fromPlayersIds">Specific player IDs if from_players is specific.</param>
        /// <param name="lastUpdate">The last update ID to poll from.</param>
        public PollUpdatesRequest(ERoomTargetPlayers fromPlayers = ERoomTargetPlayers.Host, int[] fromPlayersIds = null, string lastUpdate = null)
        {
            this.from_players = fromPlayers.ToString().ToLower();
            this.from_players_ids = fromPlayersIds;
            this.last_update = lastUpdate;
        }
    }
}
