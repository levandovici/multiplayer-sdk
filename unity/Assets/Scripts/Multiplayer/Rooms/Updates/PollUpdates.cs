using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Michitai.Multiplayer.Rooms.Updates
{
    public class PollUpdates
    {
        private ERoomTargetPlayers _from_players;
        private int[] _from_players_ids;
        private string _last_update;

        public ERoomTargetPlayers FromPlayers
        {
            get
            {
                return _from_players;
            }

            private set
            {
                _from_players = value;
            }
        }

        public int[] FromPlayersIds
        {
            get
            {
                return _from_players_ids;
            }

            private set
            {
                _from_players_ids = value;
            }
        }

        public string LastUpdate
        {
            get
            {
                return _last_update;
            }

            private set
            {
                _last_update = value;
            }
        }

        public PollUpdates(ERoomTargetPlayers fromPlayers = ERoomTargetPlayers.Host, int[] fromPlayersIds = null, string lastUpdate = null)
        {
            FromPlayers = fromPlayers;
            FromPlayersIds = fromPlayersIds;
            LastUpdate = lastUpdate;
        }
    }
}
