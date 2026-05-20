using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Michitai.Multiplayer.Rooms.Updates
{
    /// <summary>
    /// Request parameters for sending updates to players in a room.
    /// Allows targeting specific players with typed data.
    /// </summary>
    /// <typeparam name="T">The type of update data.</typeparam>
    public class UpdatePlayers<T> where T : class, new()
    {
        private ERoomTargetPlayers _target_players;
        private int[] _target_players_ids;
        private string _type;
        private T _data;

        /// <summary>
        /// Which players to send the update to.
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
        /// Specific player IDs to send the update to.
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
        /// The type of update.
        /// </summary>
        public string Type
        {
            get
            {
                return _type;
            }

            private set
            {
                _type = value;
            }
        }

        /// <summary>
        /// The update data.
        /// </summary>
        public T Data
        {
            get
            {
                return _data;
            }

            private set
            {
                _data = value;
            }
        }

        /// <summary>
        /// Initializes a new UpdatePlayers request.
        /// </summary>
        /// <param name="targetPlayers">Which players to send the update to.</param>
        /// <param name="type">The type of update.</param>
        /// <param name="data">The update data.</param>
        /// <param name="targetPlayersIds">Specific player IDs to send the update to.</param>
        public UpdatePlayers(ERoomTargetPlayers targetPlayers, string type, T data = null, int[] targetPlayersIds = null)
        {
            TargetPlayers = targetPlayers;
            TargetPlayersIds = targetPlayersIds;
            Type = type;
            Data = data;
        }
    }
}
