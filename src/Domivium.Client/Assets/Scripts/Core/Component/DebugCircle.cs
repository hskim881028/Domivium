using UnityEngine;

namespace Domivium.Client.Core.Component
{
    public class DebugCircle : MonoBehaviour
    {
        private LineRenderer _lineRenderer;
        private readonly int _segments = 64;
        private Material _material;

        private void Awake()
        {
            _lineRenderer = GetComponent<LineRenderer>();
            _lineRenderer.useWorldSpace = false;
            _lineRenderer.loop = true;
            _material = _lineRenderer.material;
        }

        public void Draw(float radius, Color color)
        {
            _material.color = color;
            _lineRenderer.positionCount = _segments;
            for (var i = 0; i < _segments; i++)
            {
                var t = 2 * Mathf.PI * i / _segments;
                _lineRenderer.SetPosition(i, new Vector3(Mathf.Cos(t) * radius, 0.1f, Mathf.Sin(t) * radius));
            }
        }
    }
}