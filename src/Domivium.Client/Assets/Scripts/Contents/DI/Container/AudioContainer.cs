using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace Domivium.Client.Contents.DI.Container
{
    [CreateAssetMenu(fileName = "AudioContainer", menuName = "ScriptableObjects/AudioContainer")]
    public class AudioContainer : ScriptableObject
    {
        [SerializeField] private List<AudioResource> _resources = new();

        public List<AudioResource> Resources => _resources;
    }
}