using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Michitai.Multiplayer.Matchmaking
{
    public class Matchmaking
    {
        public static Task<MatchmakingListResponse<T>> GetMatchmakingLobbiesAsync<T>(Client client, CancellationToken ct = default) where T : class, new()
            => client.Send<MatchmakingListResponse<T>>(HttpMethod.Get, client.Url(Endpoints.MatchmakingList), null, ct);

        public static Task<MatchmakingCreateResponse> CreateMatchmakingLobbyAsync<TPlayerData, TRules>(Client client, string playerToken, string matchmakingName, int maxPlayers = 4, bool strictFull = false,
            bool hostSwitch = false, bool canLeaveRoom = false, bool realtimeRoom = false, TPlayerData? playerData = null, TRules? rules = null, CancellationToken ct = default) where TPlayerData : class, new() where TRules : class, new()
            => client.Send<MatchmakingCreateResponse>(HttpMethod.Post, client.Url(Endpoints.MatchmakingCreate, $"&player_token={playerToken}"),
                new MatchmakingCreateRequest<TPlayerData, TRules>(matchmakingName, maxPlayers, strictFull, false, hostSwitch, canLeaveRoom, realtimeRoom, playerData, rules), ct);

        public static Task<MatchmakingCurrentResponse<T>> GetCurrentMatchmakingStatusAsync<T>(Client client, string playerToken, CancellationToken ct = default) where T : class, new()
            => client.Send<MatchmakingCurrentResponse<T>>(HttpMethod.Get, client.Url(Endpoints.MatchmakingCurrent, $"&player_token={playerToken}"), null, ct);

        public static Task<MatchmakingDirectJoinResponse> JoinMatchmakingDirectlyAsync<T>(Client client, string playerToken, string matchmakingId, T? playerData = null, CancellationToken ct = default) where T : class, new()
            => client.Send<MatchmakingDirectJoinResponse>(HttpMethod.Post, client.Url(string.Format(Endpoints.MatchmakingJoin, matchmakingId), $"&player_token={playerToken}"), playerData, ct);

        public static Task<MatchmakingLeaveResponse> LeaveMatchmakingAsync(Client client, string playerToken, CancellationToken ct = default)
            => client.Send<MatchmakingLeaveResponse>(HttpMethod.Post, client.Url(Endpoints.MatchmakingLeave, $"&player_token={playerToken}"), null, ct);

        public static Task<MatchmakingPlayersResponse<T>> GetMatchmakingPlayersAsync<T>(Client client, string playerToken, CancellationToken ct = default) where T : class, new()
            => client.Send<MatchmakingPlayersResponse<T>>(HttpMethod.Get, client.Url(Endpoints.MatchmakingPlayers, $"&player_token={playerToken}"), null, ct);

        public static Task<MatchmakingHeartbeatResponse> SendMatchmakingHeartbeatAsync(Client client, string playerToken, CancellationToken ct = default)
            => client.Send<MatchmakingHeartbeatResponse>(HttpMethod.Post, client.Url(Endpoints.MatchmakingHeartbeat, $"&player_token={playerToken}"), null, ct);

        public static Task<MatchmakingRemoveResponse> RemoveMatchmakingLobbyAsync(Client client, string playerToken, CancellationToken ct = default)
            => client.Send<MatchmakingRemoveResponse>(HttpMethod.Post, client.Url(Endpoints.MatchmakingRemove, $"&player_token={playerToken}"), null, ct);

        public static Task<MatchmakingStartResponse> StartGameFromMatchmakingAsync(Client client, string playerToken, CancellationToken ct = default)
            => client.Send<MatchmakingStartResponse>(HttpMethod.Post, client.Url(Endpoints.MatchmakingStart, $"&player_token={playerToken}"), null, ct);
    }
}
