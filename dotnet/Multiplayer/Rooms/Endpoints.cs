using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Michitai.Multiplayer.Rooms
{
    internal static class Endpoints
    {
        public const string GameRoomCreate = "game_room.php/create";
        public const string GameRoomList = "game_room.php/list";
        public const string GameRoomJoin = "game_room.php/{0}/join";
        public const string GameRoomLeave = "game_room.php/leave";
        public const string GameRoomPlayers = "game_room.php/players";
        public const string GameRoomHeartbeat = "game_room.php/heartbeat";
        public const string GameRoomActions = "game_room.php/actions";
        public const string GameRoomActionsPoll = "game_room.php/actions/poll";
        public const string GameRoomActionsPending = "game_room.php/actions/pending";
        public const string GameRoomActionComplete = "game_room.php/actions/{0}/complete";
        public const string GameRoomUpdates = "game_room.php/updates";
        public const string GameRoomUpdatesPoll = "game_room.php/updates/poll";
        public const string GameRoomCurrent = "game_room.php/current";
    }
}
