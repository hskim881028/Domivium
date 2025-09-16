using System.Collections.Generic;
using Domivium.Client.Core.UI.Presenter;

namespace Domivium.Client.Core.UI
{
    public interface IUIManager
    {
        public HashSet<UIId> GetStaticUI(UILayer layer);
        public T Get<T>(UIId id) where T : IUIPresenter;
        public void Remove(UIId id);
    }
}