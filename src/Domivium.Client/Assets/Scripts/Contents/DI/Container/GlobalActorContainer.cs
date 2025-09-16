using Domivium.Client.Core.Actors;
using UnityEngine;

namespace Domivium.Client.Contents.DI.Container
{
    [CreateAssetMenu(fileName = "GlobalActorContainer", menuName = "ScriptableObjects/GlobalActorContainer")]
    public class GlobalActorContainer : ScriptableObject
    {
        [SerializeField] private InputEventSystem _inputEventSystem;
        [SerializeField] private CameraRig _cameraRig;
        [SerializeField] private EnvironmentRig _environmentRig;
        [SerializeField] private AudioRig _audioRig;

        public InputEventSystem InputEventSystem => _inputEventSystem;

        public CameraRig CameraRig => _cameraRig;

        public EnvironmentRig EnvironmentRig => _environmentRig;

        public AudioRig AudioRig => _audioRig;
    }
}