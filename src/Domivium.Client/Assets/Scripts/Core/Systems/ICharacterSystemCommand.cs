using Domivium.Client.Core.Battle;
using UnityEngine;

namespace Domivium.Client.Core.Systems
{
    public interface ICharacterSystemCommand
    {
        public void Initialize(IBattleSystem character);
        public bool SetDirection(Vector2 value);
        public bool LookAt(Vector2 value);
        public bool Reload();
        public bool SelectItem(Vector2 value);
        public bool UseItem();
        public bool OpenInventory();
        public bool Interact();
        public bool Avoid();
    }
}