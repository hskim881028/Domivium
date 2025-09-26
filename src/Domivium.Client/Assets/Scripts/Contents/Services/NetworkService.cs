using System;
using Domivium.Client.Network;
using Domivium.Client.Network.ClientFilters;
using Domivium.Shared.Response;
using MagicOnion;

namespace Domivium.Client.Contents.Services
{
    public sealed class NetworkService
    {
        private readonly INetworkConnection _networkConnection;

        public NetworkService(INetworkConnection networkConnection)
        {
            _networkConnection = networkConnection;
        }

        public void Connect()
        {
            _networkConnection.Connect();
        }

        public void AddFilter(AuthenticationClientFilter filter)
        {
            _networkConnection.AddAuthenticationFilter(filter);
        }

        public Lazy<T> CreateService<T>() where T : IService<T> => _networkConnection.CreateService<T>();

        public bool HandleResponse(IResponse response)
        {
            this.Log($"[{nameof(IResponse)}] StatusCode: {response.StatusCode}");
            return _networkConnection.HandleResponse(response);
        }
    }
}