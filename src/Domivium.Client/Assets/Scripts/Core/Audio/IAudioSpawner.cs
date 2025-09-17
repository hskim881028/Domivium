using UnityEngine;
using UnityEngine.Audio;

namespace Domivium.Client.Core.Audio
{
    public interface IAudioSpawner
    {
        public AudioSource Get(AudioMixerGroup group, string name, float sourceVolume = 1, bool loop = false);
        public void Return(AudioSource source);
    }
}