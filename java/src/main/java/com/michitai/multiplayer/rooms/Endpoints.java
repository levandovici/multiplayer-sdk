package com.michitai.multiplayer.rooms;

/**
 * Internal class containing API endpoint constants for room-related operations.
 */
public class Endpoints {
    public static final String GAME_ROOM_CREATE = "game_room.php/create";
    public static final String GAME_ROOM_LIST = "game_room.php/list";
    public static final String GAME_ROOM_JOIN = "game_room.php/%s/join";
    public static final String GAME_ROOM_LEAVE = "game_room.php/leave";
    public static final String GAME_ROOM_PLAYERS = "game_room.php/players";
    public static final String GAME_ROOM_HEARTBEAT = "game_room.php/heartbeat";
    public static final String GAME_ROOM_CURRENT = "game_room.php/current";
    public static final String GAME_ROOM_STOP = "game_room.php/stop";
    public static final String GAME_ROOM_KICK = "game_room.php/kick";
    public static final String GAME_ROOM_PASSWORD = "game_room.php/password";
}
