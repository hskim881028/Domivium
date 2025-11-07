namespace Domivium.Client.Core.Message
{
    public enum InputMessageType
    {
        Submit,
        Cancel,
        ClickEnter,
        ClickExit,
        Point,
        
        Move,
        Look,
        LookCanceled,
        Quick,
        QuickCanceled,
        Inventory,
        Interact,
        Avoid,
    }
}