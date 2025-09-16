using UnityEngine;
using UnityEngine.Audio;

namespace Domivium.Client.Core.Actors
{
    public class AudioRig : Actor
    {
        [SerializeField] private AudioMixer _mixer;
        [SerializeField] private AudioMixerGroup _master;
        [SerializeField] private AudioMixerGroup _bgm;
        [SerializeField] private AudioMixerGroup _sfx;
        [SerializeField] private AudioMixerGroup _ui;

        public AudioMixer Mixer => _mixer;
        public AudioMixerGroup Master => _master;
        public AudioMixerGroup BGM => _bgm;
        public AudioMixerGroup SFX => _sfx;
        public AudioMixerGroup UI => _ui;
    }
}