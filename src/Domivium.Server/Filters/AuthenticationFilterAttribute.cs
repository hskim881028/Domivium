using System.Security.Claims;
using Domivium.Server.AppServices;
using Domivium.Server.Extensions;
using Domivium.Server.Utilities;
using Domivium.Shared.Common;
using Domivium.Shared.Exceptions;
using Grpc.Core;
using MagicOnion.Server;
using StatusCode = Domivium.Shared.Common.StatusCode;

namespace Domivium.Server.Filters;

public class AuthenticationFilterAttribute : MagicOnionFilterAttribute
{
    private readonly IJwtAppService _jwtService;

    public AuthenticationFilterAttribute(IJwtAppService jwtService)
    {
        _jwtService = jwtService;
    }

    public override async ValueTask Invoke(ServiceContext ctx, Func<ServiceContext, ValueTask> next)
    {
        if (AuthorizationUtils.IsAllowAnonymous(ctx))
        {
            await next(ctx);
            return;
        }

        var http = ctx.CallContext.GetHttpContext();
        var authHeader = http.Request.Headers.Authorization.ToString();

        if (string.IsNullOrWhiteSpace(authHeader) || !authHeader.StartsWith($"{GrpcAuthHeaders.Bearer} ", StringComparison.OrdinalIgnoreCase))
        {
            throw new ServerExceptions(StatusCode.Unauthenticated, "Missing or invalid Authorization header");
        }

        var token = authHeader[$"{GrpcAuthHeaders.Bearer} ".Length..].Trim();
        if (string.IsNullOrWhiteSpace(token))
        {
            throw new ServerExceptions(StatusCode.Unauthenticated, "Empty access token");
        }

        var principal = _jwtService.Validate(token);
        if (principal is null)
        {
            throw new ServerExceptions(StatusCode.Unauthenticated, "Unauthorized");
        }

        http.User = principal;

        var sub = principal.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(sub) || !Guid.TryParse(sub, out var userId))
        {
            throw new ServerExceptions(StatusCode.Unauthenticated, "Unauthorized");
        }

        ctx.SetUserId(userId);
        ctx.SetAccessToken(token);
        await next(ctx);
    }
}