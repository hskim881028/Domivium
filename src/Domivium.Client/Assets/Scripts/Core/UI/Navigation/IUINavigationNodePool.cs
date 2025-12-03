namespace Domivium.Client.Core.UI.Navigation
{
    public interface IUINavigationNodePool
    {
        public IUINavigationNode Get(UIId id);
        public void Return(IUINavigationNode node);
    }
}