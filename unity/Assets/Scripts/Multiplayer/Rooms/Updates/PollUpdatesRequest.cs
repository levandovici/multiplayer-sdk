using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Michitai.Multiplayer.Rooms.Updates
{
    [System.Serializable]
    internal class PollUpdatesRequest
    {
        public string from_players = ERoomTargetPlayers.Host.ToString().ToLower();
        public int[] from_players_ids;
        public string last_update;

        public PollUpdatesRequest(ERoomTargetPlayers fromPlayers = ERoomTargetPlayers.Host, int[]? fromPlayersIds = null, string lastUpdate = null)
        {
            this.from_players = fromPlayers.ToString().ToLower();
            this.from_players_ids = fromPlayersIds;
            this.last_update = lastUpdate;
        }
    }
}
