using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Domivium.Client.Contents.Components
{
    public class SelectTowerItem : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
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
    }
}