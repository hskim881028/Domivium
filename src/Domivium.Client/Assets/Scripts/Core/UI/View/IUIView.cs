namespace Domivium.Client.Core.UI.View
{
    public interface IUIView<in TMessage> : IUIHierarchyControl, IUIActivatable
    {
        public void AttachMessage(TMessage message);
    }
}