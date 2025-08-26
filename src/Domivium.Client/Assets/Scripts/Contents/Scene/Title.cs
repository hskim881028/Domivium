using System;
using Cysharp.Threading.Tasks;
using Domivium.Client.Contents.Services;
using Domivium.Client.Core.Scene;
using Domivium.Client.Core.Utility;
using Domivium.Client.Data.Cache;
using Domivium.Client.Network.ClientFilters;
using Domivium.Shared.Common;
using Domivium.Shared.DataTransferObject;
using Domivium.Shared.Request;
using Domivium.Shared.Services;
using UnityEngine;
using VContainer;

namespace Domivium.Client.Contents.Scene
{
    public class Title : SceneScope
    {
        private const string UserIdKey = "USER_ID_KEY";
        private AuthenticationTokenCache _tokenCache;

        protected override void Configure(IContainerBuilder builder)
        {
            base.Configure(builder);
            _tokenCache = Parent.Container.Resolve<AuthenticationTokenCache>();
            this.Log();
        }

        private void Start()
        {
            this.Log();
            Connect();
        }

        private void Connect()
        {
            if (string.IsNullOrEmpty(_tokenCache.RefreshToken))
            {
                var stringUserId = PlayerPrefs.GetString(UserIdKey, string.Empty);
                this.Log($"[Connect] userId: {stringUserId}");
                if (string.IsNullOrEmpty(stringUserId))
                {
                    RegisterAsync().Forget();
                }
                else
                {
                    LoginAsync(Guid.Parse(stringUserId)).Forget();
                }
            }
            else
            {
                RefreshTokenAsync().Forget();
            }
        }

        private async UniTaskVoid RegisterAsync()
        {
            var networkService = Parent.Container.Resolve<NetworkService>();
            var userService = networkService.CreateService<IUserService>();
            var userId = Guid.NewGuid();
            var response = await userService.Value.RegisterAsync(new RegisterRequest { UserId = userId });
            if (!networkService.HandleResponse(response)) return;

            this.Log($"[RegisterAsync] userId: {userId.ToString()}");
            PlayerPrefs.SetString(UserIdKey, userId.ToString());
            Done(networkService, response.Token);
        }

        private async UniTaskVoid LoginAsync(Guid userId)
        {
            var networkService = Parent.Container.Resolve<NetworkService>();
            var userService = networkService.CreateService<IUserService>();
            var response = await userService.Value.LoginAsync(new LoginRequest { UserId = userId });
            if (!networkService.HandleResponse(response)) return;

            Done(networkService, response.Token);
        }

        private async UniTaskVoid RefreshTokenAsync()
        {
            var networkService = Parent.Container.Resolve<NetworkService>();
            var userService = networkService.CreateService<IUserService>();
            var response = await userService.Value.RefreshTokenAsync(new RefreshTokenRequest { RefreshToken = _tokenCache.RefreshToken });
            if (!networkService.HandleResponse(response))
            {
                if (response.StatusCode != StatusCode.Unauthenticated) return;

                this.Log($"[Unauthenticated] Remove refreshToken: {_tokenCache.RefreshToken}");
                _tokenCache.Clear();
                Connect();
                return;
            }

            Done(networkService, response.Token);
        }

        private void Done(NetworkService networkService, TokenDto token)
        {
            var authenticationClientFilter = Parent.Container.Resolve<AuthenticationClientFilter>();
            networkService.AddFilter(authenticationClientFilter);

            _tokenCache.Update(token.AccessToken, token.RefreshToken, token.AccessTokenLifetimeSeconds, token.RefreshTokenLifetimeSeconds);

            var sceneService = Parent.Container.Resolve<SceneService>();
            sceneService.Load(SceneScopeIds.Lobby);
        }
    }
}