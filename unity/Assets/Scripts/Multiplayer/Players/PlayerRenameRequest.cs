using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Michitai.Multiplayer.Players
{
    [System.Serializable]
    internal class PlayerRenameRequest
    {
        public string new_name;



        public PlayerRenameRequest(string newName)
        {
            this.new_name = newName;
        }
    }
}
