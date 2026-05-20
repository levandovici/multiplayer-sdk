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
        private ERoomTargetPlayers _target_players;
        private int[] _target_players_ids;
        private string _action_type;
        private T _request_data;

        /// <summary>
        /// Which players to target with the action.
        /// </summary>
        public ERoomTargetPlayers TargetPlayers
        {
            get
            {
                return _target_players;
            }

            private set
            {
                _target_players = value;
            }
        }

        /// <summary>
        /// Specific player IDs to target with the action.
        /// </summary>
        public int[] TargetPlayersIds
        {
            get
            {
                return _target_players_ids;
            }

            private set
            {
                _target_players_ids = value;
            }
        }

        /// <summary>
        /// The type of action being submitted.
        /// </summary>
        public string ActionType
        {
            get
            {
                return _action_type;
            }

            private set
            {
                _action_type = value;
            }
        }

        /// <summary>
        /// The request data for the action.
        /// </summary>
        public T RequestData
        {
            get
            {
                return _request_data;
            }

            private set
            {
                _request_data = value;
            }
        }

        /// <summary>
        /// Initializes a new SubmitAction request.
        /// </summary>
        /// <param name="targetPlayers">Which players to target with the action.</param>
        /// <param name="type">The type of action being submitted.</param>
        /// <param name="data">The request data for the action.</param>
        /// <param name="targetPlayersIds">Specific player IDs to target with the action.</param>
        public SubmitAction(ERoomTargetPlayers targetPlayers, string type, T data = null, int[] targetPlayersIds = null)
        {
            TargetPlayers = targetPlayers;
            TargetPlayersIds = targetPlayersIds;
            ActionType = type;
            RequestData = data;
        }
    }
}
