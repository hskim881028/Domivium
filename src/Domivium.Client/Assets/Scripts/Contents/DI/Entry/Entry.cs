using Domivium.Client.Core.UI.Navigation;
using VContainer.Unity;

namespace Domivium.Client.Contents.DI.Entry
{
    public abstract class Entry : Disposable, IStartable
    {
        protected readonly IUINavigation UINavigation;

        protected Entry(IUINavigation uiNavigation)
        {
            UINavigation = uiNavigation;
        }

        protected abstract void OnStart();

        public void Start()
        {
            this.Log();
            OnStart();
        }
    }
}