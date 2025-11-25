using UnityEngine;

namespace Domivium.Client.Contents.UIComponents.ItemContainer
{
    public class Equipment : ItemContainer
    {
        [SerializeField] private GameObject _blocker;

        public override void Reset()
        {
            Unlock();
            ClearFocus();
        }

        public void Lock()
        {
            _blocker.SetActive(true);
        }

        public void Unlock()
        {
            _blocker.SetActive(false);
        }

        public void Focus(int index)
        {
            ClearFocus();
            _slots[index].Focus();
        }

        public void ClearFocus()
        {
            foreach (var equipmentSlot in _slots)
            {
                equipmentSlot.ClearFocus();
            }
        }
    }
}