using TMPro;
using UnityEngine;

namespace Domivium.Client.Core.Component
{
    public class HealthDisplay : MonoBehaviour
    {
        [SerializeField] private MeshRenderer _meshRenderer;
        [SerializeField] private TextMeshPro _text;

        private readonly int _fillShaderPropertyID = Shader.PropertyToID("_Fill");

        private Material _material;

        private void Awake()
        {
            _material = _meshRenderer.material;
        }

        public void Set(int current, int max)
        {
            var ratio = (float)current / max;
            _material.SetFloat(_fillShaderPropertyID, ratio);
            _text.text = $"{current:N0}/{max:N0}";
        }
    }
}