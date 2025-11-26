using System;
using Cysharp.Threading.Tasks;
using Domivium.Client.Contents.Services;
using Domivium.Client.Contents.UI;
using Domivium.Client.Core;
using Domivium.Client.Core.UI.Navigation;
using Domivium.Client.Data.Cache;
using Domivium.Client.Network.ClientFilters;
using Domivium.Shared.Common;
using Domivium.Shared.DataTransferObject;
using Domivium.Shared.Request;
using Domivium.Shared.Services;
using UnityEngine;

namespace Domivium.Client.Contents.DI.Entry
{
    public sealed class TitleEntry : Entry
    {
        private const string UserIdKey = "USER_ID_KEY";
        private readonly SceneService _sceneService;
        private readonly NetworkService _networkService;
        private readonly AuthenticationTokenCache _tokenCache;
        private readonly AuthenticationClientFilter _authenticationClientFilter;

        public TitleEntry(
            IUINavigation uiNavigation,
            SceneService sceneService,
            NetworkService networkService,
            AuthenticationTokenCache tokenCache,
            AuthenticationClientFilter authenticationClientFilter) : base(uiNavigation)
        {
            _sceneService = sceneService;
            _networkService = networkService;
            _tokenCache = tokenCache;
            _authenticationClientFilter = authenticationClientFilter;
        }

        protected override void OnStart()
        {
            UINavigation.ApplyUILayer(UILayers.Title).Forget();
            if (!AppEnv.LocalMode)
            {
                Connect();
            }
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
            var userService = _networkService.CreateService<IUserService>();
            var userId = Guid.NewGuid();
            var response = await userService.Value.RegisterAsync(new RegisterRequest { UserId = userId });
            if (!_networkService.HandleResponse(response)) return;

            this.Log($"[RegisterAsync] userId: {userId.ToString()}");
            PlayerPrefs.SetString(UserIdKey, userId.ToString());
            Done(_networkService, response.Token);
        }

        private async UniTaskVoid LoginAsync(Guid userId)
        {
            var userService = _networkService.CreateService<IUserService>();
            var response = await userService.Value.LoginAsync(new LoginRequest { UserId = userId });
            if (!_networkService.HandleResponse(response)) return;

            Done(_networkService, response.Token);
        }

        private async UniTaskVoid RefreshTokenAsync()
        {
            var userService = _networkService.CreateService<IUserService>();
            var response = await userService.Value.RefreshTokenAsync(new RefreshTokenRequest { RefreshToken = _tokenCache.RefreshToken });
            if (!_networkService.HandleResponse(response))
            {
                if (response.StatusCode != StatusCode.Unauthenticated) return;

                this.Log($"[Unauthenticated] Remove refreshToken: {_tokenCache.RefreshToken}");
                _tokenCache.Clear();
                Connect();
                return;
            }

            Done(_networkService, response.Token);
        }

        private void Done(NetworkService networkService, TokenDto token)
        {
            networkService.AddFilter(_authenticationClientFilter);
            _tokenCache.Update(token.AccessToken, token.RefreshToken, token.AccessTokenLifetimeSeconds, token.RefreshTokenLifetimeSeconds);

            // EnterLobby();
        }
    }
}