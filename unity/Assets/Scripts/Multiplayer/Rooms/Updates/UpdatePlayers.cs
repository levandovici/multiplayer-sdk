using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Michitai.Multiplayer.Rooms.Updates
{
    public class UpdatePlayers<T> where T : class, new()
    {
        private ERoomTargetPlayers _target_players;
        private int[] _target_players_ids;
        private string _type;
        private T _data;



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

        public string Type
        {
            get
            {
                return _type;
            }

            private set
            {
                _type = value;
            }
        }

        public T Data
        {
            get
            {
                return _data;
            }

            private set
            {
                _data = value;
            }
        }



        public UpdatePlayers(ERoomTargetPlayers targetPlayers, string type, T data = null, int[] targetPlayersIds = null)
        {
            TargetPlayers = targetPlayers;
            TargetPlayersIds = targetPlayersIds;
            Type = type;
            Data = data;
        }
    }
}
