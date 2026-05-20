using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Michitai.Multiplayer.Rooms.Actions
{
    /// <summary>
    /// Request data for submitting an action to players in a room.
    /// Serializes action data for API transmission using Unity's JsonUtility.
    /// </summary>
    [System.Serializable]
    public class ActionSubmitRequest
    {
        /// <summary>
        /// Which players to target with the action (default: All).
        /// </summary>
        public string target_players = ERoomTargetPlayers.All.ToString().ToLower();

        /// <summary>
        /// Specific player IDs to target with the action.
        /// </summary>
        public int[] target_players_ids;

        /// <summary>
        /// The type of action being submitted.
        /// </summary>
        public string action_type;

        /// <summary>
        /// Serialized request data (Unity mode).
        /// </summary>
        public string request_data_json;

        /// <summary>
        /// Initializes a new ActionSubmitRequest.
        /// </summary>
        /// <param name="targetPlayers">Which players to target with the action.</param>
        /// <param name="type">The type of action being submitted.</param>
        /// <param name="dataJson">Serialized request data.</param>
        /// <param name="targetPlayerIds">Specific player IDs to target with the action.</param>
        public ActionSubmitRequest(ERoomTargetPlayers targetPlayers, string type, string dataJson = null, int[] targetPlayerIds = null)
        {
            this.target_players = targetPlayers.ToString().ToLower();
            this.target_players_ids = targetPlayerIds;
            this.action_type = type;
            this.request_data_json = dataJson;
        }
    }
}
