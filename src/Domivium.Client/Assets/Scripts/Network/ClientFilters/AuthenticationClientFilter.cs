using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Domivium.Client.Core.Utility;
using Domivium.Client.Data.Cache;
using Domivium.Shared.Common;
using Domivium.Shared.Request;
using Domivium.Shared.Services;
using Grpc.Core;
using MagicOnion;
using MagicOnion.Client;
using StatusCode = Domivium.Shared.Common.StatusCode;

namespace Domivium.Client.Network.ClientFilters
{
    public class AuthenticationClientFilter : IClientFilter
    {
        private readonly SemaphoreSlim _refreshGate = new(1, 1);
        private readonly AuthenticationTokenCache _tokenCache;
        private GrpcChannelx _channel;

        public AuthenticationClientFilter(AuthenticationTokenCache tokenCache)
        {
            _tokenCache = tokenCache;
        }

        public async ValueTask<ResponseContext> SendAsync(RequestContext context, Func<RequestContext, ValueTask<ResponseContext>> next)
        {
            if (_tokenCache.IsRefreshTokenExpired)
            {
                throw new RpcException(new Status(Grpc.Core.StatusCode.Unauthenticated, "refresh_token_expired"));
            }

            this.Log($"_tokenCache.IsAccessTokenExpired : {_tokenCache.IsAccessTokenExpired}");
            if (_tokenCache.IsAccessTokenExpired)
            {
                await RefreshTokenAsync();
                if (context.CallOptions.Headers?.FirstOrDefault(x => x.Key == GrpcAuthHeaders.Authorization) is { } oldEntry)
                {
                    context.CallOptions.Headers.Remove(oldEntry);
                }
            }

            if (context.CallOptions.Headers != null && context.CallOptions.Headers.All(x => x.Key != GrpcAuthHeaders.Authorization))
            {
                context.CallOptions.Headers.Add(GrpcAuthHeaders.Authorization, $"{GrpcAuthHeaders.Bearer} {_tokenCache.AccessToken}");
            }

            try
            {
                return await next(context);
            }
            catch (RpcException ex) when (ex.StatusCode == Grpc.Core.StatusCode.Unauthenticated)
            {
                await RefreshTokenAsync();
                if (context.CallOptions.Headers?.FirstOrDefault(x => x.Key == GrpcAuthHeaders.Authorization) is { } oldEntry)
                {
                    context.CallOptions.Headers.Remove(oldEntry);
                }

                if (context.CallOptions.Headers != null && context.CallOptions.Headers.All(x => x.Key != GrpcAuthHeaders.Authorization))
                {
                    context.CallOptions.Headers.Add(GrpcAuthHeaders.Authorization, $"{GrpcAuthHeaders.Bearer} {_tokenCache.AccessToken}");
                }
                return await next(context);
            }
        }

        public void SetChannel(GrpcChannelx channel)
        {
            _channel = channel;
        }

        private async ValueTask RefreshTokenAsync()
        {
            await _refreshGate.WaitAsync().ConfigureAwait(false);
            try
            {
                if (!_tokenCache.IsAccessTokenExpired) return;

                var metadata = new Metadata { new Metadata.Entry(GrpcAuthHeaders.Authorization, $"{GrpcAuthHeaders.Bearer} {_tokenCache.AccessToken}") };
                var rpcService = MagicOnionClient.Create<IUserService>(_channel).WithOptions(new CallOptions(metadata));
                var response = await rpcService.RefreshTokenAsync(new RefreshTokenRequest { RefreshToken = _tokenCache.RefreshToken });
                if (response.StatusCode != StatusCode.Success)
                {
                    throw new Exception("Failed to refresh token");
                }

                var token = response.Token;
                this.Log($"[AccessToken]: {token.AccessToken}");
                this.Log($"[RefreshToken]: {token.RefreshToken}");
                _tokenCache.Update(token.AccessToken, token.RefreshToken, token.AccessTokenLifetimeSeconds, token.RefreshTokenLifetimeSeconds);
            }
            finally
            {
                _refreshGate.Release();
            }
        }
    }
}