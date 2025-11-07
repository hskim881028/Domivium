using UnityEngine;

namespace Domivium.Client.Core.Actors
{
    public sealed class CameraRig : Actor
    {
        [SerializeField] private Transform _target;
        [SerializeField] private Camera _main;
        [SerializeField] private Camera _ui;

        public Transform Target => _target;
        public Camera Main => _main;
        public Camera UI => _ui;
    }
}