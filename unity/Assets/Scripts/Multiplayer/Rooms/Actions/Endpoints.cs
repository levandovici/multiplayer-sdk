using System;
using UnityEngine;



namespace Michitai.Multiplayer.Rooms.Actions
{
    internal static class Endpoints
    {
        public const string GameRoomActions = "game_room.php/actions";
        public const string GameRoomActionsPoll = "game_room.php/actions/poll";
        public const string GameRoomActionsPending = "game_room.php/actions/pending";
        public const string GameRoomActionComplete = "game_room.php/actions/{0}/complete";
    }
}