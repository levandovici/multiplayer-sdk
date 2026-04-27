using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Michitai.Multiplayer.Rooms.Updates
{
    public class PollUpdates
    {
        public ERoomTargetPlayers From_players { get; private set; } = ERoomTargetPlayers.Host;
        public int[]? From_players_ids { get; private set; }
        public string? Last_update { get; private set; }


        public PollUpdates(ERoomTargetPlayers fromPlayers = ERoomTargetPlayers.Host, int[]? fromPlayersIds = null, string? lastUpdate = null)
        {
            From_players = fromPlayers;
            From_players_ids = fromPlayersIds;
            Last_update = lastUpdate;
        }
    }
}
