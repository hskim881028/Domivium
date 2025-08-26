using System.Threading;
using Domivium.Client.Core.UI.Contract;

namespace Domivium.Client.Core.UI.Navigation
{
    public sealed class UINavigationNode : IUINavigationNode
    {
        private readonly IUIHandle _handle;
        private CancellationTokenSource _cts = new();
        private bool _isDisposed;

        public UINavigationNode(UIId id, IUIHandle handle)
        {
            _handle = handle;
            Reset(id);
        }

        public UIId Id { get; private set; }
        public CancellationToken Token => _cts.Token;

        public void Reset(UIId id)
        {
            ResetCancellationTokenSource();
            Id = id;
            _handle.Reset(Token);
        }

        public IUIHandle Opened() => _handle.Opened();

        public void Closed(UIResult result)
        {
            _handle.Closed(result);
            Reset(Id);
        }

        public void Dispose()
        {
            if (_isDisposed) return;

            _isDisposed = true;
            _cts.Cancel();
            _cts.Dispose();
            _handle.Dispose();
        }

        private void ResetCancellationTokenSource()
        {
            if (_isDisposed) return;

            _cts.Cancel();
            _cts.Dispose();
            _cts = new CancellationTokenSource();
        }
    }
}