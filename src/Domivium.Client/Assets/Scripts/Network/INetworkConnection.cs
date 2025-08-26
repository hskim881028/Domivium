using System;
using Domivium.Client.Network.ClientFilters;
using MagicOnion;

namespace Domivium.Client.Network
{
    public interface INetworkConnection : IResponseHandler
    {
        public void Connect();
        public void AddAuthenticationFilter(AuthenticationClientFilter filter);
        public Lazy<T> CreateService<T>() where T : IService<T>;
    }
}