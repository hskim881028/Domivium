using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace Domivium.Client.Contents.Components
{
    public class Tree : MonoBehaviour
    {
        [SerializeField] private List<ShadowCaster2D> _shadowCaster2D;

        private void Awake()
        {
            foreach (var caster in _shadowCaster2D)
            {
                caster.trimEdge = 0.1f;
            }
        }
    }
}