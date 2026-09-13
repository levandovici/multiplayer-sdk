"""End-to-end demo covering the whole SDK surface.

Mirrors dotnet/Game.cs: three demos (matchmaking with join requests, direct
matchmaking join, direct room) plus time / leaderboard / game data tests.

Usage:
    python Game.py
"""

from __future__ import annotations

from dataclasses import dataclass

from multiplayer import (
    BanTime,
    Client,
    ConsoleLogger,
    RoomTargetPlayers,
)
from multiplayer.matchmaking import MatchmakingRequestAction
from multiplayer.rooms.actions import RoomCompleteActionStatus
from multiplayer.rooms.updates import PollUpdates


@dataclass
class PlayerData:
    level: int = 1
    rank: str = "Default"


@dataclass
class RulesData:
    mode: str = ""
    map: str = ""


@dataclass
class GameData:
    current_event: str = ""
    version: str = ""


@dataclass
class LocalPlayer:
    id: int
    token: str
    name: str


client = Client("YOUR_API_TOKEN", "YOUR_PRIVATE_TOKEN", logger=ConsoleLogger())
players: dict[str, LocalPlayer] = {}


def setup_players() -> None:
    print("[SETUP] Registering players...")
    for key, name in (("host", "GameHost"), ("p1", "PlayerOne"), ("p2", "PlayerTwo")):
        reg = client.players.register(name, {"level": 10, "rank": "silver"})
        players[key] = LocalPlayer(reg.player_id, reg.private_key, name)

    for p in players.values():
        client.players.authenticate(p.token, PlayerData)
        client.players.heartbeat(p.token)

    listing = client.games.get_all_players()
    print(f"[PLAYERS LIST] Total: {listing.count}")

    for p in players.values():
        data = client.players.get_data(p.token, PlayerData)
        if data.data is not None:
            data.data.level += 1
            client.players.update_data(p.token, data.data)


def cleanup() -> None:
    for p in players.values():
        client.players.logout(p.token)
    players.clear()


def room_flow(host: LocalPlayer) -> None:
    print("\n=== GAME ROOM FLOW ===")
    client.rooms.list(rules_cls=RulesData)
    current = client.rooms.get_current(host.token, RulesData)
    print(f"[ROOM] {current.room.room_name if current.room else '-'}")

    for p in players.values():
        client.actions.submit(p.token, "player_ready", {"ready": True}, RoomTargetPlayers.HOST)

    pending = client.actions.get_pending(host.token)
    print(f"[PENDING ACTIONS] {len(pending.actions)}")
    for action in pending.actions:
        client.actions.complete(
            host.token, action.action_id, RoomCompleteActionStatus.COMPLETED, {"ok": True}
        )

    client.updates.send(host.token, "game_start", {"round": 1}, RoomTargetPlayers.ALL)
    for p in players.values():
        updates = client.updates.poll(p.token)
        print(f"[UPDATES] {p.name}: {len(updates.updates)}")

    client.rooms.get_players(host.token, PlayerData)
    for p in players.values():
        client.rooms.heartbeat(p.token)
    client.rooms.stop(host.token)


def demo_matchmaking_with_requests() -> None:
    print("\n=== DEMO: MATCHMAKING WITH JOIN REQUESTS ===")
    setup_players()
    host = players["host"]

    lobby = client.matchmaking_requests.create(
        host.token, "Lobby", max_players=4, join_by_requests=True,
        player_data=PlayerData(), rules=RulesData("tdm", "arena"),
    )
    for p in (players["p1"], players["p2"]):
        req = client.matchmaking_requests.request_to_join(p.token, lobby.matchmaking_id)
        status = client.matchmaking_requests.check_status(p.token, req.request_id)
        print(f"[REQUEST] {p.name}: {status.request.status if status.request else '-'}")
        client.matchmaking_requests.respond(
            host.token, req.request_id, MatchmakingRequestAction.APPROVE
        )

    client.matchmaking.get_current(host.token, RulesData)
    client.matchmaking.get_players(host.token, PlayerData)

    start = client.matchmaking.start(host.token)
    print(f"[START] Room: {start.room_id}")
    room_flow(host)


def demo_matchmaking_direct() -> None:
    print("\n=== DEMO: MATCHMAKING DIRECT JOIN ===")
    setup_players()
    host = players["host"]

    lobby = client.matchmaking.create(host.token, "Lobby", max_players=4)
    for p in (players["p1"], players["p2"]):
        client.matchmaking.join(p.token, lobby.matchmaking_id, PlayerData())

    client.matchmaking.heartbeat(host.token)
    start = client.matchmaking.start(host.token)
    room_flow(host)


def demo_direct_room() -> None:
    print("\n=== DEMO: DIRECT ROOM ===")
    setup_players()
    host = players["host"]

    room = client.rooms.create(host.token, "Direct Battle Arena", max_players=4)
    for p in (players["p1"], players["p2"]):
        client.rooms.join(p.token, room.room_id, player_data=PlayerData())

    client.rooms.update_password(host.token, "secret")
    room_flow(host)


def common_tests() -> None:
    print("\n=== COMMON TESTS ===")

    gd = client.games.get_data(GameData)
    print(f"[GAME DATA] {gd.data}")
    client.games.update_data({"current_event": "SpringFestival", "version": "1.2.3"})

    print(f"[TIME] {client.time.get_server_time().utc}")
    print(f"[TIME+3] {client.time.get_server_time_with_offset(3).utc}")

    lb = client.leaderboard.get(["level", "wins"], limit=10, data_cls=PlayerData)
    for entry in lb.leaderboard:
        print(f"[LEADERBOARD] #{entry.rank} {entry.player_name}")


if __name__ == "__main__":
    demo_matchmaking_with_requests()
    cleanup()
    demo_matchmaking_direct()
    cleanup()
    demo_direct_room()
    cleanup()
    common_tests()
    print("\n=== All demos finished ===")
