using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Michitai.Multiplayer.Rooms
{
    /// <summary>
    /// Request data for creating a new game room.
    /// Contains room configuration and serialized player data and rules.
    /// Uses Unity's JsonUtility for serialization.
    /// </summary>
    [System.Serializable]
    internal class RoomCreateRequest
    {
        /// <summary>
        /// The name for the room.
        /// </summary>
        public string room_name;

        /// <summary>
        /// Password for the room.
        /// </summary>
        public string password;

        /// <summary>
        /// Maximum number of players allowed.
        /// </summary>
        public int max_players;

        /// <summary>
        /// Whether host switching is allowed.
        /// </summary>
        public bool host_switch;

        /// <summary>
        /// Whether the room supports realtime communication.
        /// </summary>
        public bool realtime;

        /// <summary>
        /// Serialized player data (Unity mode).
        /// </summary>
        public string player_data_json;

        /// <summary>
        /// Serialized room rules (Unity mode).
        /// </summary>
        public string rules_json;

        /// <summary>
        /// Initializes a new RoomCreateRequest.
        /// </summary>
        /// <param name="roomName">The name for the room.</param>
        /// <param name="password">Password for the room.</param>
        /// <param name="maxPlayers">Maximum number of players allowed.</param>
        /// <param name="hostSwitch">Whether host switching is allowed.</param>
        /// <param name="realtime">Whether the room supports realtime communication.</param>
        /// <param name="playerData">Serialized player data.</param>
        /// <param name="rulesJson">Serialized room rules.</param>
        public RoomCreateRequest(string roomName, string password, int maxPlayers,
            bool hostSwitch, bool realtime, string playerData, string rulesJson)
        {
            this.room_name = roomName;
            this.password = password;
            this.max_players = maxPlayers;
            this.host_switch = hostSwitch;
            this.realtime = realtime;
            this.player_data_json = playerData;
            this.rules_json = rulesJson;
        }
    }
}
