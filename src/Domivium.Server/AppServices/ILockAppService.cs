using RedLockNet;

namespace Domivium.Server.AppServices;

public interface ILockAppService
{
    public ValueTask<IRedLock> AcquireUserLockAsync(
        object userId,
        CancellationToken ct = default,
        TimeSpan? expiry = null,
        TimeSpan? wait = null,
        TimeSpan? retry = null);
}