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
        /// <summary>
        /// Which players to receive updates from (default: Host).
        /// </summary>
        public ERoomTargetPlayers From_players { get; private set; } = ERoomTargetPlayers.Host;

        /// <summary>
        /// Specific player IDs to receive updates from.
        /// </summary>
        public int[]? From_players_ids { get; private set; }

        /// <summary>
        /// Only receive updates after this update ID.
        /// </summary>
        public string? Last_update { get; private set; }

        /// <summary>
        /// Initializes a new PollUpdates request.
        /// </summary>
        /// <param name="fromPlayers">Which players to receive updates from.</param>
        /// <param name="fromPlayersIds">Specific player IDs to receive updates from.</param>
        /// <param name="lastUpdate">Only receive updates after this update ID.</param>
        public PollUpdates(ERoomTargetPlayers fromPlayers = ERoomTargetPlayers.Host, int[]? fromPlayersIds = null, string? lastUpdate = null)
        {
            From_players = fromPlayers;
            From_players_ids = fromPlayersIds;
            Last_update = lastUpdate;
        }
    }
}
