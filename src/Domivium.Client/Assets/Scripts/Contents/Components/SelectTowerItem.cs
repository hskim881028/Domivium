using Domivium.Client.Core.Component;
using UnityEngine;

namespace Domivium.Client.Contents.Components
{
    public class SelectTowerItem : MonoBehaviour
    {
        [SerializeField] private DvmButton _button;

        public DvmButton Button => _button;

        private void Awake() { }
    }
}