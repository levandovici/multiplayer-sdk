using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Michitai.Multiplayer.Matchmaking
{
    /// <summary>
    /// Request data for creating a new matchmaking lobby.
    /// Contains lobby configuration and optional player data and rules.
    /// </summary>
    /// <typeparam name="TPlayerData">The type of player data to include.</typeparam>
    /// <typeparam name="TRules">The type of lobby rules to include.</typeparam>
    public class MatchmakingCreateRequest<TPlayerData, TRules>
    where TPlayerData : class where TRules : class, new()
    {
        [JsonInclude]
        private string Matchmaking_name { get; set; } = string.Empty;
        [JsonInclude]
        private int Max_players { get; set; }
        [JsonInclude]
        private bool Strict_full { get; set; }
        [JsonInclude]
        private bool Join_by_requests { get; set; }
        [JsonInclude]
        private bool Host_switch { get; set; }
        [JsonInclude]
        private bool Can_leave_room { get; set; }
        [JsonInclude]
        private bool Realtime_room { get; set; }
        [JsonInclude]
        private string? Password { get; set; }
        [JsonInclude]
        private TPlayerData? Player_data { get; set; }
        [JsonInclude]
        private TRules? Rules { get; set; }

        /// <summary>
        /// Initializes a new MatchmakingCreateRequest.
        /// </summary>
        /// <param name="matchmakingName">The name for the matchmaking lobby.</param>
        /// <param name="maxPlayers">Maximum number of players allowed.</param>
        /// <param name="strictFull">Whether the lobby must be full to start.</param>
        /// <param name="joinByRequests">Whether players must request to join.</param>
        /// <param name="hostSwitch">Whether host switching is allowed.</param>
        /// <param name="canLeaveRoom">Whether players can leave the resulting room.</param>
        /// <param name="realtimeRoom">Whether the room supports realtime communication.</param>
        /// <param name="password">Optional password for the lobby.</param>
        /// <param name="playerData">Optional player data to include.</param>
        /// <param name="rules">Optional lobby rules to include.</param>
        public MatchmakingCreateRequest(string matchmakingName, int maxPlayers, bool strictFull,
            bool joinByRequests = false, bool hostSwitch = false, bool canLeaveRoom = false, bool realtimeRoom = false,
            string? password = null, TPlayerData? playerData = null, TRules? rules = null)
        {
            this.Matchmaking_name = matchmakingName;
            this.Max_players = maxPlayers;
            this.Strict_full = strictFull;
            this.Join_by_requests = joinByRequests;
            this.Host_switch = hostSwitch;
            this.Can_leave_room = canLeaveRoom;
            this.Realtime_room = realtimeRoom;
            this.Password = password;
            this.Player_data = playerData;
            this.Rules = rules;
        }
    }
}
