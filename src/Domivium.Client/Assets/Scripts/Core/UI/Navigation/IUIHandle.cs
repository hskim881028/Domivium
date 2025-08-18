using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Domivium.Client.Core.UI.Contract;

namespace Domivium.Client.Core.UI.Navigation
{
    public interface IUIHandle : IDisposable
    {
        public void Reset(CancellationToken cancellationToken);
        public IUIHandle Opened();
        public IUIHandle Closed(UIResult result);
        public UniTask<UIResult> WaitUntilClosedAsync();
    }
}