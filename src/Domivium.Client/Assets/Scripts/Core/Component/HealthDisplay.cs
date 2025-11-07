using UnityEngine;

namespace Domivium.Client.Core.Component
{
    public sealed class HealthDisplay : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer _renderer;

        public void Set(int current, int max)
        {
            var ratio = (float)current / max;
            _renderer.size = new Vector2(ratio, 0.16f);
        }
    }
}