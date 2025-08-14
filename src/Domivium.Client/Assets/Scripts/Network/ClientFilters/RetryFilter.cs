using System;
using System.Threading.Tasks;
using Grpc.Core;
using MagicOnion.Client;

namespace Domivium.Client.Network.ClientFilters
{
    public sealed class RetryFilter : IClientFilter
    {
        public async ValueTask<ResponseContext> SendAsync(
            RequestContext context,
            Func<RequestContext, ValueTask<ResponseContext>> next)
        {
            RpcException rpcException = null;
            Exception exception = null;
            var retryCount = 0;
            while (retryCount < 3)
            {
                try
                {
                    // using same CallOptions so be careful to add duplicate headers or etc.
                    return await next(context);
                }
                catch (RpcException ex)
                {
                    rpcException = ex;
                }
                catch (Exception ex)
                {
                    exception = ex;
                }

                retryCount++;
            }

            if (rpcException != null) throw new Exception("[Retry failed] rpcException: ", rpcException);

            throw new Exception("[Retry failed] exception:", exception);
        }
    }
}