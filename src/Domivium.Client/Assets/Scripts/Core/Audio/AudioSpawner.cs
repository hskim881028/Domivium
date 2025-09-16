using System.Collections.Generic;
using Domivium.Client.Core.Actors;
using UnityEngine;
using UnityEngine.Audio;

namespace Domivium.Client.Core.Audio
{
    public sealed class AudioSpawner : IAudioSpawner
    {
        private const int PrewarmCount = 10;

        private readonly Transform _parent;
        private readonly Queue<AudioSource> _sources = new();
        private readonly Dictionary<string, AudioResource> _resources = new();
        private int _count;

        public AudioSpawner(AudioRig audioRig, List<AudioResource> resources)
        {
            _parent = audioRig.transform;
            foreach (var resource in resources)
            {
                _resources.Add(resource.name, resource);
            }

            for (var i = 0; i < PrewarmCount; i++)
            {
                Return(CreateSource());
            }
        }

        public AudioSource Get(AudioMixerGroup group, string name, float sourceVolume = 1, bool loop = false)
        {
            var source = GetSource();
            source.enabled = true;
            source.outputAudioMixerGroup = group;
            source.resource = GetResource(name);
            source.volume = sourceVolume;
            source.loop = loop;
            return source;
        }

        public void Return(AudioSource source)
        {
            source.Stop();
            source.clip = null;
            source.outputAudioMixerGroup = null;
            source.loop = false;
            source.enabled = false;
            _sources.Enqueue(source);
        }

        private AudioResource GetResource(string name)
        {
            if (_resources.TryGetValue(name, out var resource)) return resource;

            Debug.LogError($"Key not found: {name}");
            return null;
        }

        private AudioSource GetSource() => _sources.TryDequeue(out var source) ? source : CreateSource();

        private AudioSource CreateSource()
        {
            var go = new GameObject($"{nameof(AudioSource)}_{_count}");
            go.transform.SetParent(_parent, false);
            go.transform.localPosition = Vector3.zero;
            go.transform.localRotation = Quaternion.identity;
            go.transform.localScale = Vector3.one;
            var source = go.AddComponent<AudioSource>();
            source.playOnAwake = false;
            _count++;
            return source;
        }
    }
}