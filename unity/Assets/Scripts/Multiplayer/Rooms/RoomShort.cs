using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Michitai.Multiplayer.Rooms
{
    /// <summary>
    /// Minimal information about a game room.
    /// Contains basic room details without full player information.
    /// Uses Unity's JsonUtility for deserialization of rules.
    /// </summary>
    /// <typeparam name="T">The type to deserialize room rules into.</typeparam>
    [System.Serializable]
    public class RoomShort<T> where T : class, new()
    {
        [SerializeField]
        private string rules_json;   // Unity mode

        /// <summary>
        /// The unique ID of the room.
        /// </summary>
        public string room_id;

        /// <summary>
        /// The name of the room.
        /// </summary>
        public string room_name;

        /// <summary>
        /// Maximum number of players allowed.
        /// </summary>
        public int max_players;

        /// <summary>
        /// Current number of players in the room.
        /// </summary>
        public int current_players;

        /// <summary>
        /// Whether the room has a password set.
        /// </summary>
        public bool has_password;

        /// <summary>
        /// Whether host switching is allowed.
        /// </summary>
        public bool host_switch;

        /// <summary>
        /// Whether players can leave the room.
        /// </summary>
        public bool can_leave;

        /// <summary>
        /// Whether the room supports realtime communication.
        /// </summary>
        public bool realtime;

        /// <summary>
        /// The room rules deserialized into the specified type.
        /// </summary>
        public T Rules
        {
            get
            {
                return JsonUtility.FromJson<T>(rules_json);
            }
        }
    }
}
