namespace Domivium.Client.Contents.Commands
{
    public interface IStageInventoryCommand
    {
        public void Initialize(int rerollCost, int towerLimit);
        public void Use(int slotIndex);
        public void Refill(int cost);
    }
}