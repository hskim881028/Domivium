using System;
using Domivium.Client.Network.ClientFilters;
using Domivium.Shared.Response;
using MagicOnion;
using MagicOnion.Client;

namespace Domivium.Client.Network
{
    public sealed class NetworkConnection : INetworkConnection
    {
        private readonly IClientFilter[] _clientFilters = new IClientFilter[2];
        private readonly IResponseHandler _responseHandler;

        private GrpcChannelx _channel;

        public NetworkConnection(
            IResponseHandler responseHandler,
            LoggingFilter loggingFilter,
            RetryFilter retryFilter)
        {
            _responseHandler = responseHandler;
            _clientFilters[0] = loggingFilter;
            _clientFilters[1] = retryFilter;
        }

        public void Connect()
        {
            _channel = GrpcChannelx.ForAddress("http://localhost:5000");
        }

        public Lazy<T> CreateService<T>() where T : IService<T>
        {
            return new Lazy<T>(MagicOnionClient.Create<T>(_channel, _clientFilters));
        }

        public bool HandleResponse(IResponse response)
        {
            return _responseHandler.HandleResponse(response);
        }
    }
}