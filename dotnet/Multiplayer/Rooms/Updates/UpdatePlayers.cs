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
        /// <summary>
        /// Which players to send the update to.
        /// </summary>
        public ERoomTargetPlayers Target_players { get; private set; } = ERoomTargetPlayers.All;

        /// <summary>
        /// Specific player IDs to send the update to (used when Target_players is Specific).
        /// </summary>
        public int[]? Target_players_ids { get; private set; }

        /// <summary>
        /// The type of update being sent.
        /// </summary>
        public string Type { get; private set; } = string.Empty;

        /// <summary>
        /// The update data object.
        /// </summary>
        public T? Data { get; private set; } = new();

        /// <summary>
        /// Initializes a new UpdatePlayers request.
        /// </summary>
        /// <param name="targetPlayers">Which players to send the update to.</param>
        /// <param name="type">The type of update.</param>
        /// <param name="data">The update data object.</param>
        /// <param name="targetPlayersIds">Specific player IDs to send the update to (required when targetPlayers is Specific).</param>
        public UpdatePlayers(ERoomTargetPlayers targetPlayers, string type, T? data = null, int[]? targetPlayersIds = null)
        {
            Target_players = targetPlayers;
            Target_players_ids = targetPlayersIds;
            Type = type;
            Data = data;
        }
    }
}
