namespace Domivium.Server.Data;

public interface IPostCommitQueue
{
    public void Enqueue(Func<CancellationToken, Task> work);
    public List<Func<CancellationToken, Task>> Drain();
    public void Clear();
}