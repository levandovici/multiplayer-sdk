using Michitai.Multiplayer.Errors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Michitai.Multiplayer.Rooms.Actions
{
    public class ActionSubmitResponse : ApiResponse<ERoomActionsError>
    {
        public int Actions_sent { get; set; }
        public List<string> Action_ids { get; set; } = new();
        public List<int> Target_players_ids { get; set; } = new();
    }
}
