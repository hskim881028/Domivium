using System;
using Domivium.Client.Core.Actors;
using Domivium.Client.Core.Message;
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

            BindAction("Player/Move", performed: Move, canceled: Move);
            BindAction("Player/Look", performed: Look, canceled: LookEnd);
            BindAction("Player/Quick", performed: Quick, canceled: QuickEnd);
            BindAction("Player/Inventory", performed: Inventory);
            BindAction("Player/Interact", performed: Interact);
            BindAction("Player/Avoid", performed: Avoid);
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
        }

        private void Click(InputAction.CallbackContext context)
        {
            if (context.action.WasPressedThisFrame())
            {
                _publisher.Publish(InputMessage.ClickEnter(_pointer));
            }

            if (context.action.WasReleasedThisFrame())
            {
                _publisher.Publish(InputMessage.ClickExit(_pointer));
            }
        }

        private void Point(InputAction.CallbackContext context)
        {
            _pointer = context.ReadValue<Vector2>();
            _publisher.Publish(InputMessage.Point(_pointer));
        }

        private void Move(InputAction.CallbackContext context)
        {
            var value = context.ReadValue<Vector2>();
            _publisher.Publish(InputMessage.Move(value));
        }

        private void Look(InputAction.CallbackContext context)
        {
            var value = context.ReadValue<Vector2>();
            _publisher.Publish(InputMessage.Look(value));
        }

        private void LookEnd(InputAction.CallbackContext context)
        {
            _publisher.Publish(InputMessage.LookCanceled);
        }

        private void Quick(InputAction.CallbackContext context)
        {
            var value = context.ReadValue<Vector2>();
            _publisher.Publish(InputMessage.Look(value));
        }

        private void QuickEnd(InputAction.CallbackContext context)
        {
            var value = context.ReadValue<Vector2>();
            _publisher.Publish(InputMessage.Look(value));
        }

        private void Inventory(InputAction.CallbackContext context)
        {
            _publisher.Publish(InputMessage.Inventory);
        }

        private void Interact(InputAction.CallbackContext context)
        {
            _publisher.Publish(InputMessage.Interact);
        }

        private void Avoid(InputAction.CallbackContext context)
        {
            _publisher.Publish(InputMessage.Avoid);
        }
    }
}