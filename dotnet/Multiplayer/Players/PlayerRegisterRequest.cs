using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Michitai.Multiplayer.Players
{
    /// <summary>
    /// Request data for registering a new player.
    /// Contains player name and optional player data.
    /// </summary>
    /// <typeparam name="T">The type of player data to include.</typeparam>
    internal class PlayerRegisterRequest<T> where T : class, new()
    {
        [JsonInclude]
        internal required string Player_name { get; set; }
        [JsonInclude]
        private T? Player_data { get; set; }
        [JsonInclude]
        internal string? Steam_ticket { get; set; }
        [JsonInclude]
        internal string? Steam_identity { get; set; }
        [JsonInclude]
        internal string? Google_auth_code { get; set; }
        [JsonInclude]
        internal string? Google_integrity_token { get; set; }
        [JsonInclude]
        internal bool Restore { get; set; }

        /// <summary>
        /// Initializes a new PlayerRegisterRequest.
        /// </summary>
        /// <param name="playerName">The player's display name.</param>
        /// <param name="playerData">Optional player data to include.</param>
        /// <param name="steamTicket">Optional hex-encoded Steam auth ticket.</param>
        /// <param name="steamIdentity">Optional Steam ticket identity string.</param>
        /// <param name="googleAuthCode">Optional Play Games server auth code.</param>
        /// <param name="googleIntegrityToken">Optional Play Integrity token for the ownership check.</param>
        /// <param name="restore">Explicit user consent to cancel a pending account deletion. Send only after a restore prompt is confirmed; never set automatically.</param>
        [SetsRequiredMembers]
        public PlayerRegisterRequest(string playerName, T? playerData = null, string? steamTicket = null, string? steamIdentity = null, string? googleAuthCode = null, string? googleIntegrityToken = null, bool restore = false)
        {
            this.Player_name = playerName;
            this.Player_data = playerData;
            this.Steam_ticket = steamTicket;
            this.Steam_identity = steamIdentity;
            this.Google_auth_code = googleAuthCode;
            this.Google_integrity_token = googleIntegrityToken;
            this.Restore = restore;
        }
    }
}
