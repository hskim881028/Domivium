using System;
using Domivium.Client.Core.Component;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Domivium.Client.Contents.Components
{
    public class SelectTowerItem : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        [SerializeField] private CostText _soul;
        [SerializeField] private GameObject _towerContainer;
        [SerializeField] private GameObject _dimmed;

        public Action PointerDown { get; set; }
        public Action PointerUp { get; set; }

        public void OnPointerDown(PointerEventData eventData)
        {
            PointerDown?.Invoke();
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            PointerUp?.Invoke();
        }

        public void Use()
        {
            _towerContainer.SetActive(false);
            _dimmed.SetActive(false);
            _soul.Cost = 0;
        }

        public void Refill(int cost)
        {
            _towerContainer.SetActive(true);
            _soul.Cost = cost;
        }

        public void SetDimmed(bool value)
        {
            if (!_towerContainer.activeSelf) return;

            _dimmed.SetActive(value);
        }
    }
}