using System;
using Domivium.Client.Core.Actors;
using Domivium.Client.Core.Message;
using Domivium.Client.Core.Systems;
using MessagePipe;
using R3;
using UnityEngine;

namespace Domivium.Client.Contents.Systems
{
    public sealed class CameraSystem : Disposable, ICameraSystem, ICameraSystemCommand
    {
        private readonly CameraRig _cameraRig;
        private readonly IActorManager _actorManager;

        public Camera MainCamera => _cameraRig.Main;
        public Camera UICamera => _cameraRig.UI;

        public CameraSystem(
            CameraRig cameraRig,
            IActorManager actorManager,
            ISubscriber<SceneMessage> sceneSubscriber)
        {
            _cameraRig = cameraRig;
            actorManager.Character.Subscribe(OnChangeCharacter).AddTo(ref DisposableBag);
            sceneSubscriber.Subscribe(OnSceneMessage).AddTo(ref DisposableBag);
        }

        private void OnChangeCharacter(IUnitPresenter character)
        {
            if (character == null)
            {
                _cameraRig.Target.SetParent(_cameraRig.transform);
            }
            else
            {
                _cameraRig.Target.SetParent(character.Transform);
                _cameraRig.Target.localPosition = Vector3.back;
                _cameraRig.Target.localRotation = Quaternion.identity;
            }
        }

        private void OnSceneMessage(SceneMessage message)
        {
            switch (message.Type)
            {
                case SceneMessageType.Unload:
                    _cameraRig.Target.SetParent(_cameraRig.transform);
                    break;
                case SceneMessageType.Load:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}