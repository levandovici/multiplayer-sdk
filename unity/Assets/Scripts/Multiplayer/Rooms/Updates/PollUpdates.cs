using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Michitai.Multiplayer.Rooms.Updates
{
    /// <summary>
    /// Request parameters for polling updates in a room.
    /// Allows filtering updates by source and time.
    /// </summary>
    public class PollUpdates
    {
        private ERoomTargetPlayers _from_players;
        private int[] _from_players_ids;
        private string _last_update;

        /// <summary>
        /// Which players to receive updates from.
        /// </summary>
        public ERoomTargetPlayers FromPlayers
        {
            get
            {
                return _from_players;
            }

            private set
            {
                _from_players = value;
            }
        }

        /// <summary>
        /// Specific player IDs to receive updates from.
        /// </summary>
        public int[] FromPlayersIds
        {
            get
            {
                return _from_players_ids;
            }

            private set
            {
                _from_players_ids = value;
            }
        }

        /// <summary>
        /// Only receive updates after this update ID.
        /// </summary>
        public string LastUpdate
        {
            get
            {
                return _last_update;
            }

            private set
            {
                _last_update = value;
            }
        }

        /// <summary>
        /// Initializes a new PollUpdates request.
        /// </summary>
        /// <param name="fromPlayers">Which players to receive updates from.</param>
        /// <param name="fromPlayersIds">Specific player IDs to receive updates from.</param>
        /// <param name="lastUpdate">Only receive updates after this update ID.</param>
        public PollUpdates(ERoomTargetPlayers fromPlayers = ERoomTargetPlayers.Host, int[] fromPlayersIds = null, string lastUpdate = null)
        {
            FromPlayers = fromPlayers;
            FromPlayersIds = fromPlayersIds;
            LastUpdate = lastUpdate;
        }
    }
}
