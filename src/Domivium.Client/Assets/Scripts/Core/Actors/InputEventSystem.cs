using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using VContainer;

namespace Domivium.Client.Core.Actors
{
    public sealed class InputEventSystem : Actor
    {
        private PointerEventData _eventData;
        private readonly List<RaycastResult> _hits = new();

        public InputSystemUIInputModule InputModule { get; private set; }

        public EventSystem EventSystem { get; private set; }

        [Inject]
        public void Construct()
        {
            InputModule = GetComponentInChildren<InputSystemUIInputModule>();
            EventSystem = GetComponentInChildren<EventSystem>();
            _eventData = new PointerEventData(EventSystem);
        }

        public bool IsPointerOverUI(Vector2 position)
        {
            _eventData.Reset();
            _eventData.position = position;
            _hits.Clear();
            EventSystem.RaycastAll(_eventData, _hits);
            return _hits.Count > 0;
        }
    }
}