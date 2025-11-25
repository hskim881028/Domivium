using Cysharp.Threading.Tasks;
using Domivium.Client.Core.UI.Contract;

namespace Domivium.Client.Core.UI.Navigation
{
    public interface IUINavigation
    {
        public bool IsRunning { get; }
        public bool HasOpenSystemUI { get; }
        public bool HasOpenStackUI { get; }
        public bool IsTopOfStack(UIId id);
        public UniTask ApplyUILayer(UILayer layer, bool immediately = false);
        public UniTask<IUIHandle> ShowStackUIAsync(UIId id, UIParam payload = null, bool immediately = false);
        public UniTask<IUIHandle> ShowSystemUIAsync(UIId id, UIParam payload = null, bool immediately = false);
        public UniTask<bool> HideStackUIAsync(UIResult result, bool immediately = false);
        public UniTask<bool> HideSystemUIAsync(UIId id, UIResult result, bool immediately = false);
    }
}