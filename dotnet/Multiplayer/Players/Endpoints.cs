using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Michitai.Multiplayer.Players
{
    internal static class Endpoints
    {
        public const string GamePlayersRegister = "game_players.php/register";
        public const string GamePlayersLogin = "game_players.php/login";
        public const string GamePlayersHeartbeat = "game_players.php/heartbeat";
        public const string GamePlayersLogout = "game_players.php/logout";
        public const string GamePlayersRename = "game_players.php/rename";

        public const string GameDataPlayerGet = "game_data.php/player/get";
        public const string GameDataPlayerUpdate = "game_data.php/player/update";
    }
}

