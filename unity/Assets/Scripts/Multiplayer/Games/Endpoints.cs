using System;
using UnityEngine;



namespace Michitai.Multiplayer.Games
{
    /// <summary>
    /// Internal static class containing API endpoint constants for game-related operations.
    /// </summary>
    internal static class Endpoints
    {
        /// <summary>
        /// Endpoint for listing all players in a game.
        /// </summary>
        public const string GamePlayersList = "game_players.php/list";

        /// <summary>
        /// Endpoint for retrieving global game data.
        /// </summary>
        public const string GameDataGameGet = "game_data.php/game/get";

        /// <summary>
        /// Endpoint for updating global game data.
        /// </summary>
        public const string GameDataGameUpdate = "game_data.php/game/update";
    }
}