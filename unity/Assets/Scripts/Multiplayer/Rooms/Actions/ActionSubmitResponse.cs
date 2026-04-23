using Michitai.Multiplayer.Errors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Michitai.Multiplayer.Rooms.Actions
{
    [System.Serializable]
    public class ActionSubmitResponse : ApiResponse<ERoomActionsError>
    {
        public int actions_sent;
        public List<string> action_ids = new();
        public List<int> target_players_ids = new();
    }
}
