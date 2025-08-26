namespace Domivium.Server.Data;

public sealed class PostCommitQueue : IPostCommitQueue
{
    private readonly List<Func<CancellationToken, Task>> _items = new();

    public void Enqueue(Func<CancellationToken, Task> work)
    {
        _items.Add(work);
    }

    public List<Func<CancellationToken, Task>> Drain()
    {
        var copy = new List<Func<CancellationToken, Task>>(_items);
        _items.Clear();
        return copy;
    }

    public void Clear()
    {
        _items.Clear();
    }
}