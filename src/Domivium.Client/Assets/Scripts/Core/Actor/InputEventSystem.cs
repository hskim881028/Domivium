using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using VContainer;

namespace Domivium.Client.Core.Actor
{
    public class InputEventSystem : Actor
    {
        public InputSystemUIInputModule InputModule { get; private set; }
        public EventSystem EventSystem { get; private set; }

        [Inject]
        public void Construct()
        {
            InputModule = GetComponentInChildren<InputSystemUIInputModule>();
            EventSystem = GetComponentInChildren<EventSystem>();
        }
    }
}