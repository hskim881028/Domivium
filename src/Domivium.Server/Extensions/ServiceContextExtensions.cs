using MagicOnion.Server;

namespace Domivium.Server.Extensions;

public static class ServiceContextExtensions
{
    private const string UserIdKey = "UserId";
    private const string AccessTokenKey = "AccessToken";

    public static Guid GetUserId(this ServiceContext ctx)
    {
        return ctx.Items.TryGetValue(UserIdKey, out var v) && v is Guid id ? id : Guid.Empty;
    }

    public static string GetAccessToken(this ServiceContext ctx)
    {
        return ctx.Items.TryGetValue(AccessTokenKey, out var v) && v is string s ? s : string.Empty;
    }

    public static void SetUserId(this ServiceContext ctx, Guid id)
    {
        ctx.Items[UserIdKey] = id;
    }

    public static void SetAccessToken(this ServiceContext ctx, string token)
    {
        ctx.Items[AccessTokenKey] = token;
    }

    public static void Clear(this ServiceContext ctx)
    {
        ctx.Items.TryRemove(UserIdKey, out _);
        ctx.Items.TryRemove(AccessTokenKey, out _);
    }
}