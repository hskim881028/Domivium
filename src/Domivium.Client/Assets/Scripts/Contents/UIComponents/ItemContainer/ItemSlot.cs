using System;
using Domivium.Client.Data.Item;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Domivium.Client.Contents.UIComponents.ItemContainer
{
    public class ItemSlot : MonoBehaviour, IPointerClickHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        [SerializeField] private Image _frameImage;
        [SerializeField] private ItemVisual _itemVisual;
        [SerializeField] private ItemSlotType _type;
        [SerializeField] private int _index;

        public ItemSlotEntry SlotInfo => new(_type, _index);
        public Sprite Sprite => _itemVisual.Item;
        public int Count => _itemVisual.Count;
        public bool IsStackable => _itemVisual.IsStackable;
        public RectTransform RectTransform { get; private set; }
        public Action<ItemSlotEntry> SelectItem { get; set; }
        public Action<ItemSlotEntry, Vector2> PickItem { get; set; }
        public Action<Vector2> MoveItem { get; set; }
        public Action<ItemSlotEntry, Vector2> DropItem { get; set; }

        private void Awake()
        {
            RectTransform = GetComponent<RectTransform>();
        }

        public void Set(Sprite sprite, int count, bool isStackable)
        {
            _itemVisual.Show(sprite, count, isStackable);
        }

        public void Clear()
        {
            _itemVisual.Hide();
        }

        public void Focus()
        {
            _frameImage.color = new Color(1, 0.8707691f, 0, 0.8627451f);
        }

        public void ClearFocus()
        {
            _frameImage.color = new Color(0.009f, 0.05f, 0.078f, 0.8509804f);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            SelectItem?.Invoke(SlotInfo);
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            PickItem?.Invoke(SlotInfo, eventData.position);
        }

        public void OnDrag(PointerEventData eventData)
        {
            MoveItem?.Invoke(eventData.position);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            DropItem?.Invoke(SlotInfo, eventData.position);
        }
    }
}