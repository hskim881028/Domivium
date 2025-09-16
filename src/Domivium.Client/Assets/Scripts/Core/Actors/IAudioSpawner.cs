using System;
using UnityEngine;
using UnityEngine.Audio;

namespace Domivium.Client.Core.Actors
{
    public interface IAudioSpawner : IDisposable
    {
        public AudioSource Get(AudioMixerGroup group, string name, float sourceVolume = 1, bool loop = false);
        public void Return(AudioSource source);
    }
}