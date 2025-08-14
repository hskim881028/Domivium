using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Domivium.Client.Core.Utility;
using MagicOnion.Client;

namespace Domivium.Client.Network.ClientFilters
{
    public sealed class LoggingFilter : IClientFilter
    {
        public async ValueTask<ResponseContext> SendAsync(
            RequestContext context,
            Func<RequestContext, ValueTask<ResponseContext>> next)
        {
            ZLog.Request(context);
            var sw = Stopwatch.StartNew();
            var response = await next(context);
            sw.Stop();
            ZLog.Response(context.MethodPath, sw.Elapsed.TotalMilliseconds, response);
            return response;
        }
    }
}