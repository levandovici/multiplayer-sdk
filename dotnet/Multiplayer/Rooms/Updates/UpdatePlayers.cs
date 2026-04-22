using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Michitai.Multiplayer.Rooms.Updates
{
    public class UpdatePlayers<T> where T : class, new()
    {
        public ERoomTargetPlayers Target_players { get; private set; } = ERoomTargetPlayers.All;
        public int[]? Target_players_ids { get; private set; }
        public string Type { get; private set; } = string.Empty;
        public T? Data { get; private set; } = new();



        public UpdatePlayers(ERoomTargetPlayers targetPlayers, string type, T? data = null, int[]? targetPlayersIds = null)
        {
            Target_players = targetPlayers;
            Target_players_ids = targetPlayersIds;
            Type = type;
            Data = data;
        }
    }
}
