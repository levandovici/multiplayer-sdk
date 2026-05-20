using UnityEngine;

namespace Michitai.Multiplayer.Players
{
    /// <summary>
    /// Ban duration options for player bans.
    /// Defines the time periods a player can be banned from the game.
    /// </summary>
    public enum EBanTime
    {
        /// <summary>1 hour ban duration.</summary>
        Hour,
        /// <summary>1 day ban duration.</summary>
        Day,
        /// <summary>1 week ban duration.</summary>
        Week,
        /// <summary>1 month ban duration.</summary>
        Month,
        /// <summary>3 months (quarter) ban duration.</summary>
        Quarter,
        /// <summary>1 year ban duration.</summary>
        Year,
        /// <summary>Permanent ban with no expiration.</summary>
        Forever
    }
}
