package com.michitai.multiplayer.players;

/**
 * Ban duration options for player bans.
 * Defines the time periods a player can be banned from the game.
 */
public enum EBanTime {
    /** 1 hour ban duration. */
    HOUR,
    /** 1 day ban duration. */
    DAY,
    /** 1 week ban duration. */
    WEEK,
    /** 1 month ban duration. */
    MONTH,
    /** 3 months (quarter) ban duration. */
    QUARTER,
    /** 1 year ban duration. */
    YEAR,
    /** Permanent ban with no expiration. */
    FOREVER
}
