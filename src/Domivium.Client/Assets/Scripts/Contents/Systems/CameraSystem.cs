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

        public Camera MainCamera => _cameraRig.Main;
        public Camera UICamera => _cameraRig.UI;

        public CameraSystem(CameraRig cameraRig, ISubscriber<SceneMessage> sceneSubscriber)
        {
            _cameraRig = cameraRig;
            sceneSubscriber.Subscribe(OnSceneMessage).AddTo(ref DisposableBag);
        }

        public void Initialize(Transform character)
        {
            _cameraRig.Target.SetParent(character);
            _cameraRig.Target.localPosition = Vector3.back;
            _cameraRig.Target.localRotation = Quaternion.identity;
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