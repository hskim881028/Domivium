namespace Domivium.Client.Core.UI.Presenter
{
    public interface IStaticUIPresenter : IUIPresenter
    {
        public UIPriority Priority { get; }
        public bool HasLayer(UILayer layer);
    }
}