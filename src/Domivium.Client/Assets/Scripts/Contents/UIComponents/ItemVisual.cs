using Domivium.Client.Core.Component;
using UnityEngine;
using UnityEngine.UI;

namespace Domivium.Client.Contents.UIComponents
{
    public class ItemVisual : MonoBehaviour
    {
        [SerializeField] private Image _itemImage;
        [SerializeField] private IntText _countText;
        [SerializeField] private GameObject _countObject;

        private RectTransform _rectTransform;

        public Sprite Item => _itemImage.sprite;
        public int Count => _countText.Value;
        public bool IsStackable => _countObject.activeSelf;

        private void Awake()
        {
            _rectTransform = GetComponent<RectTransform>();
        }

        public void Show(Sprite sprite)
        {
            gameObject.SetActive(true);
            _itemImage.sprite = sprite;
            _countText.Value = 1;
            _countObject.SetActive(false);
        }

        public void Show(Sprite sprite, int count)
        {
            gameObject.SetActive(true);
            _itemImage.sprite = sprite;
            _countObject.SetActive(true);
            _countText.Value = count;
        }

        public void Show(Sprite sprite, int count, bool isStackable)
        {
            gameObject.SetActive(count > 0);
            _itemImage.sprite = sprite;
            _countObject.SetActive(isStackable);
            _countText.Value = count;
        }

        public void Hide()
        {
            gameObject.SetActive(false);
            _countText.Value = 0;
            _countObject.SetActive(false);
        }

        public void SetPosition(Vector2 position)
        {
            _rectTransform.anchoredPosition = position;
        }
    }
}