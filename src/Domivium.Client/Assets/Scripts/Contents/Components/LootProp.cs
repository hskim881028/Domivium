using System.Collections.Generic;
using Domivium.Client.Data.Loot;
using UnityEngine;

namespace Domivium.Client.Contents.Components
{
    public class LootProp : MonoBehaviour
    {
        private const float StartX = -2.34f;
        private const float SizeX = 0.26f;

        [SerializeField] private Transform _axis;
        [SerializeField] private SpriteRenderer[] _numbers;
        [SerializeField] private Sprite[] _sprites;
        [SerializeField] private LootType _lootType;
        [SerializeField] [Range(0, 100)] private ushort _lootId;

        private readonly Queue<int> _cached = new();

        public LootType LootType => _lootType;
        public ushort LootId => _lootId;

        private void OnEnable()
        {
            _axis.gameObject.SetActive(false);
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            SetText();
        }
#endif

        private void SetText()
        {
            if (_numbers == null) return;

            _cached.Clear();
            var cur = _lootId;
            while (cur >= 10)
            {
                var d = cur % 10;
                _cached.Enqueue(d);
                cur /= 10;
            }

            _cached.Enqueue(cur);

            Reset();

            var index = 0;
            while (_cached.Count > 0)
            {
                var num = _cached.Dequeue();
                _numbers[index].gameObject.SetActive(true);
                _numbers[index].sprite = _sprites[num];
                index++;
            }

            _axis.localPosition = new Vector3(StartX + (index - 1) * SizeX, 0, 0);
        }

        private void Reset()
        {
            if (_numbers == null) return;

            foreach (var number in _numbers)
            {
                number.gameObject.SetActive(false);
            }
        }
    }
}