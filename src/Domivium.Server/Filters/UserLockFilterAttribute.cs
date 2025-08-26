using Domivium.Server.AppServices;
using Domivium.Server.Extensions;
using Domivium.Server.Utilities;
using Grpc.Core;
using MagicOnion.Server;

public class UserLockFilterAttribute : MagicOnionFilterAttribute
{
    private readonly ILockAppService _lockAppService;
    private readonly ILogger<UserLockFilterAttribute> _logger;

    public UserLockFilterAttribute(ILockAppService lockAppService, ILogger<UserLockFilterAttribute> logger)
    {
        _lockAppService = lockAppService;
        _logger = logger;
    }

    public override async ValueTask Invoke(ServiceContext ctx, Func<ServiceContext, ValueTask> next)
    {
        if (AuthorizationUtils.IsAllowAnonymous(ctx))
        {
            await next(ctx);
            return;
        }

        var userId = ctx.GetUserId();
        if (userId == Guid.Empty)
        {
            await next(ctx);
            return;
        }

        var ct = ctx.CallContext.CancellationToken;
        await using var redLock = await _lockAppService.AcquireUserLockAsync(userId, ct);
        if (!redLock.IsAcquired)
        {
            _logger.LogWarning("Failed to acquire user lock. userId={UserId}", userId);
            throw new RpcException(new Status(StatusCode.Aborted, "Distributed Lock Failed"));
        }

        await next(ctx);
    }
}