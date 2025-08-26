using Domivium.Shared.Common;
using Domivium.Shared.Exceptions;
using Domivium.Shared.Response;
using MagicOnion.Server;
using Microsoft.IdentityModel.Tokens;

namespace Domivium.Server.Filters;

public class ExceptionFilterAttribute : MagicOnionFilterAttribute
{
    public override async ValueTask Invoke(ServiceContext ctx, Func<ServiceContext, ValueTask> next)
    {
        try
        {
            await next(ctx);
        }
        catch (Exception e)
        {
            var statusCode = e switch
            {
                ServerExceptions serverExceptions => serverExceptions.StatusCode,
                SecurityTokenException => StatusCode.Unauthenticated,
                _ => int.MinValue
            };

            var methodInfo = ctx.MethodInfo;
            var resultType = methodInfo.ReturnType.GetGenericArguments()[0];
            if (Activator.CreateInstance(resultType) is IResponse response)
            {
                response.StatusCode = statusCode;
                ctx.SetRawResponse(response);
            }
        }
    }
}