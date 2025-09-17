using UnityEngine;

namespace Domivium.Client.Core.Actors
{
    public sealed class CameraRig : Actor
    {
        [SerializeField] private Camera _main;
        [SerializeField] private Camera _vfx;
        [SerializeField] private Camera _ui;

        public Camera Main => _main;
        public Camera VFX => _vfx;
        public Camera UI => _ui;
    }
}