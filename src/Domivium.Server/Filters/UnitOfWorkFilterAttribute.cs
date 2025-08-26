using Domivium.Server.Data;
using Domivium.Shared.Common;
using Domivium.Shared.Response;
using MagicOnion.Server;

namespace Domivium.Server.Filters;

public sealed class UnitOfWorkFilterAttribute : MagicOnionFilterAttribute
{
    public override async ValueTask Invoke(ServiceContext ctx, Func<ServiceContext, ValueTask> next)
    {
        await next(ctx);

        if (ctx.GetRawResponse() is not IResponse { StatusCode: StatusCode.Success }) return;

        var db = ctx.ServiceProvider.GetService<AppDbContext>();
        if (db is null) return;

        if (!db.ChangeTracker.HasChanges()) return;

        await db.SaveChangesAsync(ctx.CallContext.CancellationToken);
    }
}