using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Michitai.Multiplayer.Rooms.Updates
{
    /// <summary>
    /// Request data for sending updates to players in a room.
    /// Serializes update data for API transmission.
    /// </summary>
    /// <typeparam name="T">The type of update data.</typeparam>
    internal class UpdatePlayersRequest<T> where T : class, new()
    {
        [JsonInclude]
        private string Target_players { get; set; } = ERoomTargetPlayers.All.ToString().ToLower();
        [JsonInclude]
        private int[]? Target_players_ids { get; set; }
        [JsonInclude]
        private string Type { get; set; } = string.Empty;
        [JsonInclude]
        private T? Data { get; set; } = new();

        /// <summary>
        /// Initializes a new UpdatePlayersRequest.
        /// </summary>
        /// <param name="targetPlayers">Which players to send the update to.</param>
        /// <param name="type">The type of update.</param>
        /// <param name="data">The update data.</param>
        /// <param name="targetPlayersIds">Specific player IDs to send the update to.</param>
        public UpdatePlayersRequest(ERoomTargetPlayers targetPlayers, string type, T? data = null, int[]? targetPlayersIds = null)
        {
            Target_players = targetPlayers.ToString().ToLower();
            Target_players_ids = targetPlayersIds;
            Type = type;
            Data = data;
        }
    }
}
