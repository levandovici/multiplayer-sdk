using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Michitai.Multiplayer.Rooms.Actions
{
    public class SubmitAction<T> where T : class, new()
    {
        private ERoomTargetPlayers _target_players;
        private int[] _target_players_ids;
        private string _action_type;
        private T _request_data;



        public ERoomTargetPlayers TargetPlayers
        {
            get
            {
                return _target_players;
            }

            private set
            {
                _target_players = value;
            }
        }

        public int[] TargetPlayersIds
        {
            get
            {
                return _target_players_ids;
            }

            private set
            {
                _target_players_ids = value;
            }
        }

        public string ActionType
        {
            get
            {
                return _action_type;
            }

            private set
            {
                _action_type = value;
            }
        }

        public T RequestData
        {
            get
            {
                return _request_data;
            }

            private set
            {
                _request_data = value;
            }
        }



        public SubmitAction(ERoomTargetPlayers targetPlayers, string type, T data = null, int[] targetPlayersIds = null)
        {
            TargetPlayers = targetPlayers;
            TargetPlayersIds = targetPlayersIds;
            ActionType = type;
            RequestData = data;
        }
    }
}
