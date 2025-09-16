using System.Collections.Generic;
using Domivium.Client.Core.Actors;
using UnityEngine;
using UnityEngine.Audio;
using DisposableBag = R3.DisposableBag;

namespace Domivium.Client.Core.Audio
{
    public sealed class AudioSpawner : IAudioSpawner
    {
        private readonly Transform _parent;
        private DisposableBag _disposable;
        private bool _isDisposed;

        private readonly Queue<AudioSource> _sources = new();
        private readonly Dictionary<string, AudioResource> _resources = new();

        public AudioSpawner(AudioRig audioRig, List<AudioResource> resources)
        {
            _parent = audioRig.transform;
            foreach (var resource in resources)
            {
                _resources.Add(resource.name, resource);
            }
        }

        public AudioSource Get(AudioMixerGroup group, string name, float sourceVolume = 1, bool loop = false)
        {
            var source = GetSource();
            source.outputAudioMixerGroup = group;
            source.resource = GetResource(name);
            source.volume = sourceVolume;
            source.loop = loop;
            return source;
        }

        public void Return(AudioSource source)
        {
            _sources.Enqueue(source);
        }

        public void Dispose()
        {
            if (_isDisposed) return;

            _isDisposed = true;
            _disposable.Dispose();
        }

        private AudioResource GetResource(string name)
        {
            if (_resources.TryGetValue(name, out var resource)) return resource;

            Debug.LogError($"Key not found: {name}");
            return null;
        }

        private AudioSource GetSource()
        {
            return _sources.TryDequeue(out var source) ? source : CreateSource();
        }

        private AudioSource CreateSource()
        {
            var go = new GameObject();
            go.transform.SetParent(_parent, false);
            go.transform.localPosition = Vector3.zero;
            go.transform.localRotation = Quaternion.identity;
            go.transform.localScale = Vector3.one;

            var source = go.AddComponent<AudioSource>();
            source.playOnAwake = false;
            return source;
        }
    }
}