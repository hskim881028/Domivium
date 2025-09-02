using Domivium.Client.Contents.ReadModels;
using Domivium.Client.Core.Actors;
using UnityEngine;

namespace Domivium.Client.Contents.Services
{
    public sealed class CameraService : ICameraReadModel
    {
        private readonly CameraRig _cameraRig;

        public Camera MainCamera => _cameraRig.MainCamera;

        public Camera UICamera => _cameraRig.UICamera;

        public CameraService(CameraRig cameraRig)
        {
            _cameraRig = cameraRig;
        }

        public void SetBackground(Color color)
        {
            MainCamera.backgroundColor = color;
        }
    }
}