namespace Domivium.Client.Core.UI
{
    public interface IUIBehaviour
    {
        public void OnShowEnter();
        public void OnShowExit();
        public void OnHideEnter();
        public void OnHideExit();
    }
}