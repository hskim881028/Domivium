using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Domivium.Server.Data;

public sealed class PostCommitRedisInterceptor : SaveChangesInterceptor
{
    private readonly IPostCommitQueue _queue;
    private readonly ILogger<PostCommitRedisInterceptor> _logger;

    public PostCommitRedisInterceptor(IPostCommitQueue queue, ILogger<PostCommitRedisInterceptor> logger)
    {
        _queue = queue;
        _logger = logger;
    }

    public override async ValueTask<int> SavedChangesAsync(
        SaveChangesCompletedEventData eventData,
        int result,
        CancellationToken cancellationToken = default)
    {
        var tasks = _queue.Drain();
        foreach (var t in tasks)
        {
            try
            {
                await t(cancellationToken);
            }
            catch (Exception ex)
            {
                // 캐시 반영 실패는 DB 롤백 대상이 아니므로 로깅 후 지속
                _logger.LogError(ex, "Post-commit task failed.");
            }
        }
        return await base.SavedChangesAsync(eventData, result, cancellationToken);
    }

    public override void SaveChangesFailed(DbContextErrorEventData eventData)
    {
        _queue.Clear();
        base.SaveChangesFailed(eventData);
    }

    public override Task SaveChangesFailedAsync(DbContextErrorEventData eventData, CancellationToken cancellationToken = default)
    {
        _queue.Clear();
        return base.SaveChangesFailedAsync(eventData, cancellationToken);
    }
}