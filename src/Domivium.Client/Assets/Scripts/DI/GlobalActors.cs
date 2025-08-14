using Domivium.Client.Core.Actor;
using UnityEngine;

namespace Domivium.Client.DI
{
    [CreateAssetMenu(fileName = "GlobalActors", menuName = "ScriptableObjects/GlobalActors")]
    public class GlobalActors : ScriptableObject
    {
        [SerializeField] private InputEventSystem _inputEventSystem;
        [SerializeField] private CameraRig _cameraRig;
        [SerializeField] private EnvironmentRig _environmentRig;

        public InputEventSystem InputEventSystem => _inputEventSystem;

        public CameraRig CameraRig => _cameraRig;

        public EnvironmentRig EnvironmentRig => _environmentRig;
    }
}