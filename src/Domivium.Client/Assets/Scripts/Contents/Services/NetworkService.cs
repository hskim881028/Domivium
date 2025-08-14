using System;
using Domivium.Client.Network;
using Domivium.Shared.Response;
using MagicOnion;

namespace Domivium.Client.Contents.Services
{
    public sealed class NetworkService : IDisposable
    {
        private readonly INetworkConnection _networkConnection;

        public NetworkService(INetworkConnection networkConnection)
        {
            _networkConnection = networkConnection;
        }
        public void Dispose() { }

        public void Connect()
        {
            _networkConnection.Connect();
        }

        public Lazy<T> CreateService<T>() where T : IService<T>
        {
            return _networkConnection.CreateService<T>();
        }

        public bool HandleResponse(IResponse response)
        {
            return _networkConnection.HandleResponse(response);
        }
    }
}