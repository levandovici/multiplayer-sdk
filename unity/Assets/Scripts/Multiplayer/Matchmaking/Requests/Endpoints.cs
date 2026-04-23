using System;
using UnityEngine;



namespace Michitai.Multiplayer.Matchmaking.Requests
{
    internal static class Endpoints
    {
        public const string MatchmakingCreate = "matchmaking.php/create";
        public const string MatchmakingRequest = "matchmaking.php/{0}/request";
        public const string MatchmakingResponse = "matchmaking.php/{0}/response";
        public const string MatchmakingRequestStatus = "matchmaking.php/{0}/status";
    }
}