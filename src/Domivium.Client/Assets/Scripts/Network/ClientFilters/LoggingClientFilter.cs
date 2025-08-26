using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Domivium.Client.Core.Utility;
using MagicOnion.Client;

namespace Domivium.Client.Network.ClientFilters
{
    public sealed class LoggingClientFilter : IClientFilter
    {
        public async ValueTask<ResponseContext> SendAsync(
            RequestContext ctx,
            Func<RequestContext, ValueTask<ResponseContext>> next)
        {
            ZLog.Request(ctx);
            var sw = Stopwatch.StartNew();
            var response = await next(ctx);
            sw.Stop();
            ZLog.Response(ctx.MethodPath, sw.Elapsed.TotalMilliseconds, response);
            return response;
        }
    }
}