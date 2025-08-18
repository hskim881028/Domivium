using System;
using System.Threading;
using Domivium.Client.Core.UI.Contract;

namespace Domivium.Client.Core.UI.Navigation
{
    public interface IUINavigationNode : IDisposable
    {
        public UIId Id { get; }
        public CancellationToken Token { get; }
        public void Reset(UIId id);
        public IUIHandle Opened();
        public void Closed(UIResult result);
    }
}