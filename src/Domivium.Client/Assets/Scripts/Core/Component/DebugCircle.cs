using UnityEngine;

namespace Domivium.Client.Core.Component
{
    public class DebugCircle : MonoBehaviour
    {
        private static readonly int TintId = Shader.PropertyToID("_Tint");
        private const int Segments = 64;

        private LineRenderer _lineRenderer;
        private MaterialPropertyBlock _materialPropertyBlock;

        private void Awake()
        {
            _lineRenderer = GetComponent<LineRenderer>();
            _lineRenderer.useWorldSpace = false;
            _lineRenderer.loop = true;
        }

        public void Draw(float radius, Color color)
        {
            var materialPropertyBlock = new MaterialPropertyBlock();
            materialPropertyBlock.SetColor(TintId, color);
            _lineRenderer.SetPropertyBlock(materialPropertyBlock);
            _lineRenderer.positionCount = Segments;
            for (var i = 0; i < Segments; i++)
            {
                var t = 2 * Mathf.PI * i / Segments;
                _lineRenderer.SetPosition(i, new Vector3(Mathf.Cos(t) * radius, 0.1f, Mathf.Sin(t) * radius));
            }
        }
    }
}