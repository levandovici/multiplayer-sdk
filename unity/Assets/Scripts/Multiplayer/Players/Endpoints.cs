using System;
using UnityEngine;



namespace Michitai.Multiplayer.Players
{
    internal static class Endpoints
    {
        public const string GamePlayersRegister = "game_players.php/register";
        public const string GamePlayersLogin = "game_players.php/login";
        public const string GamePlayersHeartbeat = "game_players.php/heartbeat";
        public const string GamePlayersLogout = "game_players.php/logout";
        public const string GamePlayersRename = "game_players.php/rename";
        public const string GamePlayersList = "game_players.php/list";

        public const string GameDataPlayerGet = "game_data.php/player/get";
        public const string GameDataPlayerUpdate = "game_data.php/player/update";
    }
}