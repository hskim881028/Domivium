using DG.Tweening;
using UnityEngine;

namespace Domivium.Client.Contents.Components
{
    public class SpriteNumber : MonoBehaviour
    {
        private SpriteRenderer _renderer;
        private Sequence _sequence;

        private void Awake()
        {
            _renderer = GetComponent<SpriteRenderer>();
            _sequence = DOTween.Sequence()
                .SetAutoKill(false)
                .Pause()
                .Append(_renderer.DOFade(0f, 0.5f).SetDelay(0.4f).SetRecyclable(true));
        }

        private void OnDestroy()
        {
            _sequence?.Kill();
        }

        public void Show(Sprite sprite, Color color)
        {
            gameObject.SetActive(true);
            _renderer.sprite = sprite;
            _renderer.color = color;
            _sequence.Rewind();
            _sequence.Play();
        }

        public void Hide()
        {
            _sequence.Rewind();
            _sequence.Pause();
            gameObject.SetActive(false);
        }
    }
}