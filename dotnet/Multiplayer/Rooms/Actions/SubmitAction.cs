using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Michitai.Multiplayer.Rooms.Actions
{
    public class SubmitAction<T> where T : class, new()
    {
        public ERoomTargetPlayers Target_players { get; private set; } = ERoomTargetPlayers.All;
        public int[]? Target_players_ids { get; private set; }
        public string Action_type { get; private set; } = string.Empty;
        public T? Request_data { get; private set; }



        public SubmitAction(ERoomTargetPlayers targetPlayers, string type, T? data = null, int[]? targetPlayersIds = null)
        {
            Target_players = targetPlayers;
            Target_players_ids = targetPlayersIds;
            Action_type = type;
            Request_data = data;
        }
    }
}
