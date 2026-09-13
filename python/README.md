# Michitai Multiplayer API — Python SDK

Official Python client for the [Multiplayer API](https://api.michitai.com) —
player accounts, game rooms, matchmaking, leaderboards, game data storage,
server time sync and realtime (WebSocket) updates.

## Install

Download `SDK.zip` from the [Python SDK page](https://api.michitai.com/python/index.php)
and extract it next to your game code, or install this directory with pip:

```bash
pip install .            # core SDK (requires `requests`)
pip install .[realtime]  # + `websockets` for realtime rooms
```

## Quick start

```python
from multiplayer import Client

client = Client("YOUR_API_TOKEN", "YOUR_PRIVATE_TOKEN")

# Register & authenticate a player
reg = client.players.register("PlayerOne", {"level": 1, "class": "mage"})
player_token = reg.private_key

auth = client.players.authenticate(player_token)
print(auth.player.player_name)

# Rooms
room = client.rooms.create(player_token, "Lobby 1", max_players=4)
client.rooms.heartbeat(player_token)

# Matchmaking, leaderboard, time, game data — see Game.py
```

## API surface

| Service | Access | Covers |
|---|---|---|
| `client.players` | `game_players.php`, `game_data.php/player/*` | register, authenticate, heartbeat, logout, rename, ban/unban, get/update player data |
| `client.rooms` | `game_room.php/*` | create, list, join, leave, players, heartbeat, current, stop, kick, password |
| `client.actions` | `game_room.php/actions*` | submit, poll, pending, complete |
| `client.updates` | `game_room.php/updates*` | send updates, poll updates |
| `client.matchmaking` | `matchmaking.php/*` | list, create, current, direct join, leave, players, heartbeat, remove, start, stop, kick, password |
| `client.matchmaking_requests` | `matchmaking.php/*/request|response|status` | approval-based join flow |
| `client.leaderboard` | `leaderboard.php` | ranked queries on player-data fields |
| `client.games` | `game_players.php/list`, `game_data.php/game/*` | list all players, get/update global game data |
| `client.time` | `time.php` | server UTC time, optional offset |
| `Realtime` | `realtime.php/token` + `wss://realtime.michitai.com` | token + asyncio WebSocket client |

All responses are dataclasses extending `ApiResponse` — check `response.success`,
`response.error`, `response.error_type` (typed enum) and `response.error_message`.
Optional `data_cls`/`rules_cls` arguments deserialize nested payloads
(`player_data`, `rules`, `data`, …) into your own dataclasses.

## Realtime (async)

```python
from multiplayer.rooms.realtime import Realtime, RoomTargetPlayer

rt = Realtime()
token = rt.get_token(client, player_token)
await rt.connect(token.token)

rt.on_receive = lambda cmd, data, sender: print(cmd, data, sender.player_name)
await rt.send(RoomTargetPlayer.ALL, "move", {"x": 1, "y": 2})
```

## License

MIT No Attribution (MIT-0) — see `../LICENSE`.
