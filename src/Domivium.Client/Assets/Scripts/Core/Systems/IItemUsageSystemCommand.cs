namespace Domivium.Client.Core.Systems
{
    public interface IItemUsageSystemCommand
    {
        public bool UseProjectile();
        public void Reload(int capacity);
    }
}