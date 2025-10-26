using Cysharp.Threading.Tasks;
using Domivium.Client.Contents.Commands;
using Domivium.Client.Contents.Context;
using Domivium.Client.Contents.ReadModels;
using Domivium.Client.Core.Actors;
using Domivium.Client.Core.Director;
using Domivium.Client.Core.Provider;
using Domivium.Client.Core.Utility;
using UnityEngine;

namespace Domivium.Client.Contents.Services
{
    public sealed class CameraService : ICameraReadModel, ICameraCommand
    {
        private Plane _dragPlane = new(Vector3.up, Vector3.zero);

        private readonly CameraRig _cameraRig;
        private readonly StageFieldProvider _stageFieldProvider;
        private readonly IStageDirector _director;
        private Vector3 _position;
        private bool _isPressed;
        private int _limitX;
        private int _limitZ;

        public Camera MainCamera => _cameraRig.Main;
        public Camera UICamera => _cameraRig.UI;

        public CameraService(
            CameraRig cameraRig,
            StageFieldProvider stageFieldProvider,
            IStageDirector director)
        {
            _cameraRig = cameraRig;
            _stageFieldProvider = stageFieldProvider;
            _director = director;
        }

        public void SetBackground(Color color)
        {
            MainCamera.backgroundColor = color;
        }

        public void Initialize(int stageId)
        {
            var biome = _stageFieldProvider.Get(stageId);
            _limitX = biome.cellBounds.xMax / 2;
            _limitZ = biome.cellBounds.yMax / 2;
        }

        public bool MoveStarted(Vector2 position)
        {
            _dragPlane = new Plane(Vector3.up, Vector3.zero);
            if (!TryScreenToWorld(position, out var worldPosition)) return false;

            _isPressed = true;
            _position = worldPosition;

            _dragPlane = new Plane(Vector3.up, worldPosition);
            return _director.TrySetMode(StageModes.MoveCamera);
        }

        public bool UpdatePosition(Vector2 position)
        {
            if (!_isPressed) return false;

            if (!TryScreenToWorld(position, out var worldPosition)) return false;

            var diff = _position - worldPosition;
            var target = _cameraRig.Target;
            var x = Mathf.Clamp(target.position.x + diff.x, -_limitX, _limitX);
            var z = Mathf.Clamp(target.position.z + diff.z, -_limitZ - 3, _limitZ - 6);
            _cameraRig.Target.position = new Vector3(x, _cameraRig.Target.position.y, z);
            _position = worldPosition;
            return true;
        }

        public bool MoveEnd(Vector2 position)
        {
            _position = Vector3.zero;
            _isPressed = false;
            return _director.TrySetMode(StageModes.Battle);
        }

        public bool TryScreenToWorld(Vector2 screenPosition, out Vector3 worldPosition)
        {
            worldPosition = Vector3.zero;
            var ray = MainCamera.ScreenPointToRay(screenPosition);
            if (!_dragPlane.Raycast(ray, out var hit)) return false;

            worldPosition = ray.GetPoint(hit);
            return true;
        }

        public bool TryScreenToCollider(Vector2 screenPosition, out Collider collider)
        {
            collider = null;

            var ray = MainCamera.ScreenPointToRay(screenPosition);
            if (!Physics.Raycast(ray, out var hit, Mathf.Infinity, Layer.CharacterMask)) return false;

            collider = hit.collider;
            return true;
        }
    }
}