using UnityEngine;

namespace Domivium.Client.Contents.Commands
{
    public interface IStageInventoryCommand
    {
        public void Initialize(int stageId, int startSoul, int rerollCost, int towerLimit);
        public void RefillTower(int cost);
        public void BuildTower(int slotIndex);
        public void UpgradeTower(int slotIndex, Vector3Int cell);
        public void Restrict(Vector3Int cell);
    }
}