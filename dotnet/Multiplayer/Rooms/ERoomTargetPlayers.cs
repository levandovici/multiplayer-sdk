using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Michitai.Multiplayer.Rooms
{
    /// <summary>
    /// Specifies the target players for room actions and updates.
    /// </summary>
    public enum ERoomTargetPlayers
    {
        /// <summary>Target only the room host.</summary>
        Host,
        /// <summary>Target all players in the room including sender.</summary>
        All,
        /// <summary>Target all players except the sender.</summary>
        Others,
        /// <summary>Target specific player IDs (must be specified in request).</summary>
        Specific
    }
}
