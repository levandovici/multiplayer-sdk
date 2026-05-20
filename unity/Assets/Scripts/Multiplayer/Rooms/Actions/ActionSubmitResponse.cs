using Michitai.Multiplayer.Errors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Michitai.Multiplayer.Rooms.Actions
{
    /// <summary>
    /// Response returned when actions are successfully submitted to target players.
    /// Contains the action IDs and target player information.
    /// </summary>
    [System.Serializable]
    public class ActionSubmitResponse : ApiResponse<ERoomActionsError>
    {
        /// <summary>
        /// The number of actions that were sent.
        /// </summary>
        public int actions_sent;

        /// <summary>
        /// List of IDs for the submitted actions.
        /// </summary>
        public List<string> action_ids = new();

        /// <summary>
        /// List of target player IDs for the actions.
        /// </summary>
        public List<int> target_players_ids = new();
    }
}
