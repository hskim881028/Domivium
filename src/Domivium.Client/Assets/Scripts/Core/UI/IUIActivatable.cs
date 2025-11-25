using System.Threading;
using Cysharp.Threading.Tasks;
using Domivium.Client.Core.UI.Contract;

namespace Domivium.Client.Core.UI
{
    public interface IUIActivatable
    {
        public UniTask<bool> InitializeAsync(CancellationToken token);
        public UniTask ShowAsync(CancellationToken token, UIParam param, bool immediately = false);
        public UniTask HideAsync(CancellationToken token, bool immediately = false);
        public void Activate(float opacity = 1);
        public void Deactivate();
    }
}