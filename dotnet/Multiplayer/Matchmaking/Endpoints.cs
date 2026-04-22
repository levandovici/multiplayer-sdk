using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Michitai.Multiplayer.Matchmaking
{
    internal static class Endpoints
    {
        public const string MatchmakingList = "matchmaking.php/list";
        public const string MatchmakingCreate = "matchmaking.php/create";
        public const string MatchmakingCurrent = "matchmaking.php/current";
        public const string MatchmakingJoin = "matchmaking.php/{0}/join";
        public const string MatchmakingLeave = "matchmaking.php/leave";
        public const string MatchmakingPlayers = "matchmaking.php/players";
        public const string MatchmakingHeartbeat = "matchmaking.php/heartbeat";
        public const string MatchmakingRemove = "matchmaking.php/remove";
        public const string MatchmakingStart = "matchmaking.php/start";
    }
}
