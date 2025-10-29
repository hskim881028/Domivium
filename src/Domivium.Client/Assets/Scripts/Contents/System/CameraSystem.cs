using Domivium.Client.Contents.System.Command;
using Domivium.Client.Contents.System.Model;
using Domivium.Client.Core.Actors;
using UnityEngine;

namespace Domivium.Client.Contents.System
{
    public sealed class CameraSystem : Disposable, ICameraSystemModel, ICameraSystemCommand
    {
        private readonly CameraRig _cameraRig;

        public Camera MainCamera => _cameraRig.Main;
        public Camera UICamera => _cameraRig.UI;

        public CameraSystem(CameraRig cameraRig)
        {
            _cameraRig = cameraRig;
        }

        public void Initialize(Transform character)
        {
            _cameraRig.Target.SetParent(character);
            _cameraRig.Target.localPosition = Vector3.back;
            _cameraRig.Target.localRotation = Quaternion.identity;
        }
    }
}