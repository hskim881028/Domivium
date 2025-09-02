using System;
using Domivium.Client.Core.Actors;
using Domivium.Client.Core.Message;
using Domivium.Client.Core.Utility;
using MessagePipe;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Domivium.Client.Core.Input
{
    public sealed class InputDispatcher
    {
        private readonly InputEventSystem _inputEventSystem;
        private readonly IPublisher<InputMessage> _publisher;
        private Vector2 _pointer;

        public InputDispatcher(InputEventSystem inputEventSystem, IPublisher<InputMessage> publisher)
        {
            _inputEventSystem = inputEventSystem;
            _publisher = publisher;
            BindAction("UI/Submit", performed: Submit);
            BindAction("UI/Cancel", performed: Cancel);
            BindAction("UI/Click", performed: Click);
            BindAction("UI/Point", performed: Point);
        }

        private void BindAction(
            string actionName,
            Action<InputAction.CallbackContext> started = null,
            Action<InputAction.CallbackContext> performed = null,
            Action<InputAction.CallbackContext> canceled = null)
        {
            var action = _inputEventSystem.InputModule.actionsAsset.FindAction(actionName);
            if (action == null)
            {
                this.Log($"Action '{actionName}' not found.");
                return;
            }

            if (started != null)
            {
                action.started += started;
            }

            if (performed != null)
            {
                action.performed += performed;
            }

            if (canceled != null)
            {
                action.canceled += canceled;
            }


            action.Enable();
        }

        private void Submit(InputAction.CallbackContext context)
        {
            _publisher.Publish(InputMessage.Submit);
        }

        private void Cancel(InputAction.CallbackContext context)
        {
            _publisher.Publish(InputMessage.Cancel);

            // var currentSelected = _inputEventSystem.EventSystem.currentSelectedGameObject;
            // if (currentSelected == null) return;

            // var selectable = currentSelected.GetComponent<IUISelectable>();
            // if (selectable != null)
            // {
            //     // TODO: Need to implement
            // }
        }

        private void Click(InputAction.CallbackContext context)
        {
            if (context.action.WasPressedThisFrame()) return;

            // var currentSelected = _inputEventSystem.EventSystem.currentSelectedGameObject;
            // if (currentSelected != null) return;

            _publisher.Publish(InputMessage.Click(_pointer));
        }

        private void Point(InputAction.CallbackContext context)
        {
            _pointer = context.ReadValue<Vector2>();
            _publisher.Publish(InputMessage.Point(_pointer));
        }
    }
}