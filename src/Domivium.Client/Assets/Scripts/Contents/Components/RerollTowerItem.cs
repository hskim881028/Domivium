using Domivium.Client.Core.Component;
using UnityEngine;

namespace Domivium.Client.Contents.Components
{
    public class RerollTowerItem : MonoBehaviour
    {
        [SerializeField] private CostButton _button;
        [SerializeField] private GameObject _dimmed;

        public DvmButton Button => _button;

        public void SetCost(int cost)
        {
            _button.Cost = cost;
            _dimmed.SetActive(true);
        }

        public void SetDimmed(bool value)
        {
            _dimmed.SetActive(value);
        }
    }
}