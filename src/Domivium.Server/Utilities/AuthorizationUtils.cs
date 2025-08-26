using System.Collections.Concurrent;
using System.Reflection;
using Grpc.Core;
using MagicOnion.Server;
using Microsoft.AspNetCore.Authorization;

namespace Domivium.Server.Utilities;

internal static class AuthorizationUtils
{
    private static readonly ConcurrentDictionary<MethodInfo, bool> MethodAllowCache = new();
    private static readonly ConcurrentDictionary<Type, bool> TypeAllowCache = new();

    public static bool IsAllowAnonymous(ServiceContext? ctx)
    {
        if (ctx is null)
        {
            return false;
        }

        var methodInfo = ctx.MethodInfo;
        if (!MethodAllowCache.TryGetValue(methodInfo, out var methodAllow))
        {
            methodAllow = methodInfo.IsDefined(typeof(AllowAnonymousAttribute), true);
            MethodAllowCache[methodInfo] = methodAllow;
        }

        if (methodAllow)
        {
            return true;
        }

        var serviceType = ctx.ServiceType;
        if (!TypeAllowCache.TryGetValue(serviceType, out var typeAllow))
        {
            typeAllow = serviceType.IsDefined(typeof(AllowAnonymousAttribute), true);
            TypeAllowCache[serviceType] = typeAllow;
        }

        if (typeAllow)
        {
            return true;
        }

        var endpoint = ctx.CallContext.GetHttpContext().GetEndpoint();
        return endpoint?.Metadata.GetMetadata<IAllowAnonymous>() is not null;
    }
}