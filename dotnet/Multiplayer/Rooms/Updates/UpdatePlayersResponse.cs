using Michitai.Multiplayer.Errors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Michitai.Multiplayer.Rooms.Updates
{
    public class UpdatePlayersResponse : ApiResponse<ERoomUpdatesError>
    {
        public int Updates_sent { get; set; }
        public List<string> Update_ids { get; set; } = new();
        public List<int> Target_players_ids { get; set; } = new();
    }
}
