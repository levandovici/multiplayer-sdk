using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Michitai.Multiplayer.Rooms.Actions
{
    /// <summary>
    /// Request parameters for submitting an action to players in a room.
    /// Allows targeting specific players with typed action data.
    /// </summary>
    /// <typeparam name="T">The type of request data.</typeparam>
    public class SubmitAction<T> where T : class, new()
    {
        /// <summary>
        /// Which players to target with the action (default: All).
        /// </summary>
        public ERoomTargetPlayers Target_players { get; private set; } = ERoomTargetPlayers.All;

        /// <summary>
        /// Specific player IDs to target with the action.
        /// </summary>
        public int[]? Target_players_ids { get; private set; }

        /// <summary>
        /// The type of action being submitted.
        /// </summary>
        public string Action_type { get; private set; } = string.Empty;

        /// <summary>
        /// The request data for the action.
        /// </summary>
        public T? Request_data { get; private set; }

        /// <summary>
        /// Initializes a new SubmitAction request.
        /// </summary>
        /// <param name="targetPlayers">Which players to target with the action.</param>
        /// <param name="type">The type of action being submitted.</param>
        /// <param name="data">The request data for the action.</param>
        /// <param name="targetPlayersIds">Specific player IDs to target with the action.</param>
        public SubmitAction(ERoomTargetPlayers targetPlayers, string type, T? data = null, int[]? targetPlayersIds = null)
        {
            Target_players = targetPlayers;
            Target_players_ids = targetPlayersIds;
            Action_type = type;
            Request_data = data;
        }
    }
}
