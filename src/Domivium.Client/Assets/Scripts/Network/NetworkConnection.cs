using System;
using System.Collections.Generic;
using Domivium.Client.Core;
using Domivium.Client.Core.Utility;
using Domivium.Client.Network.ClientFilters;
using Domivium.Shared.Response;
using MagicOnion;
using MagicOnion.Client;
using UnityEngine;

namespace Domivium.Client.Network
{
    public sealed class NetworkConnection : INetworkConnection
    {
        private readonly IResponseHandler _responseHandler;
        private IClientFilter[] _clientFilters;
        private GrpcChannelx _channel;

        public NetworkConnection(IResponseHandler responseHandler, LoggingClientFilter loggingClientFilter)
        {
            _responseHandler = responseHandler;
            _clientFilters = new IClientFilter[1];
            _clientFilters[0] = loggingClientFilter;
        }

        public void Connect()
        {
            _channel = GrpcChannelx.ForAddress(AppEnv.ServerUrl);
        }

        public void AddAuthenticationFilter(AuthenticationClientFilter filter)
        {
            this.Log();
            filter.SetChannel(_channel);

            var filters = new List<IClientFilter>();
            filters.AddRange(_clientFilters);
            filters.Add(filter);
            _clientFilters = filters.ToArray();
        }

        public Lazy<T> CreateService<T>() where T : IService<T> => new(MagicOnionClient.Create<T>(_channel, _clientFilters));

        public bool HandleResponse(IResponse response) => _responseHandler.HandleResponse(response);
    }
}