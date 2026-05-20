using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Michitai.Multiplayer.Rooms.Updates
{
    /// <summary>
    /// Internal request data for sending player updates in Unity.
    /// Uses serialized JSON string for data.
    /// </summary>
    [System.Serializable]
    internal class UpdatePlayersRequest
    {
        /// <summary>
        /// The target players for the update (all, host, others, specific).
        /// </summary>
        public string target_players = ERoomTargetPlayers.All.ToString().ToLower();

        /// <summary>
        /// Specific player IDs if target_players is specific.
        /// </summary>
        public int[] target_players_ids;

        /// <summary>
        /// The type of update being sent.
        /// </summary>
        public string type;

        /// <summary>
        /// Serialized JSON string of the update data (Unity mode).
        /// </summary>
        public string data_json;

        /// <summary>
        /// Initializes a new UpdatePlayersRequest.
        /// </summary>
        /// <param name="targetPlayers">The target players for the update.</param>
        /// <param name="type">The type of update being sent.</param>
        /// <param name="dataJson">Serialized JSON string of the update data.</param>
        /// <param name="targetPlayerIds">Specific player IDs if target_players is specific.</param>
        public UpdatePlayersRequest(ERoomTargetPlayers targetPlayers, string type, string dataJson = null, int[] targetPlayerIds = null)
        {
            this.target_players = targetPlayers.ToString().ToLower();
            this.target_players_ids = targetPlayerIds;
            this.type = type;
            this.data_json = dataJson;
        }
    }
}
