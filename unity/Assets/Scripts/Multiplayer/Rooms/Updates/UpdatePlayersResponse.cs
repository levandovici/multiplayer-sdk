using Michitai.Multiplayer.Errors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Michitai.Multiplayer.Rooms.Updates
{
    [System.Serializable]
    public class UpdatePlayersResponse : ApiResponse<ERoomUpdatesError>
    {
        public int updates_sent;
        public List<string> update_ids = new();
        public List<int> target_players_ids = new();
    }
}
