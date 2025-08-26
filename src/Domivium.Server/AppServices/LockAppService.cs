using RedLockNet;
using RedLockNet.SERedis;

namespace Domivium.Server.AppServices;

public class LockAppService : ILockAppService, IDisposable
{
    private static readonly TimeSpan DefaultExpiry = TimeSpan.FromSeconds(30);
    private static readonly TimeSpan DefaultWait = TimeSpan.FromSeconds(8);
    private static readonly TimeSpan DefaultRetry = TimeSpan.FromMilliseconds(200);

    private readonly RedLockFactory _factory;
    private bool _disposed;

    public LockAppService(RedLockFactory factory)
    {
        _factory = factory;
    }

    public async ValueTask<IRedLock> AcquireUserLockAsync(
        object userId,
        CancellationToken ct = default,
        TimeSpan? expiry = null,
        TimeSpan? wait = null,
        TimeSpan? retry = null)
    {
        var e = expiry ?? DefaultExpiry;
        var w = wait ?? DefaultWait;
        var r = retry ?? Jitter(DefaultRetry);

        return await _factory.CreateLockAsync(UserLockKey(userId), e, w, r, ct);
    }

    public void Dispose()
    {
        if (_disposed) return;

        _disposed = true;
        _factory.Dispose();
    }

    private static string UserLockKey(object userId)
    {
        return $"user:lock:{userId}";
    }

    private static TimeSpan Jitter(TimeSpan baseRetry)
    {
        var ms = baseRetry.TotalMilliseconds;
        var rnd = Random.Shared.NextDouble() * ms; // 0 ~ base
        var sign = Random.Shared.Next(0, 2) == 0 ? -1 : 1;
        var jitter = ms + sign * rnd * 0.5;
        return TimeSpan.FromMilliseconds(Math.Max(10, jitter));
    }
}