using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Domivium.Client.Core.Actors;
using Domivium.Client.Core.Audio;
using Domivium.Client.Core.Message;
using MessagePipe;
using R3;
using UnityEngine;
using UnityEngine.Audio;

namespace Domivium.Client.Contents.Audio
{
    public sealed class AudioPlayer : Disposable, IAudioPlayer
    {
        private const string MuteKey = "_MUTE_KEY";
        private const string VolumeKey = "_VOLUME_KEY";

        private readonly AudioRig _audioRig;
        private readonly IReadOnlyDictionary<AudioId, string> _names;
        private readonly IAudioSpawner _spawner;

        private readonly List<AudioSource> _sfx = new();
        private AudioSource _bgm;
        private AudioSource _ui;

        private CancellationTokenSource _cts = new();

        private readonly Dictionary<AudioParam, bool> _groupMutes = new()
        {
            { AudioParam.Master, PlayerPrefs.GetInt($"{AudioParam.Master}{MuteKey}", 0) > 0 },
            { AudioParam.BGM, PlayerPrefs.GetInt($"{AudioParam.BGM}{MuteKey}", 0) > 0 },
            { AudioParam.SFX, PlayerPrefs.GetInt($"{AudioParam.SFX}{MuteKey}", 0) > 0 },
            { AudioParam.UI, PlayerPrefs.GetInt($"{AudioParam.UI}{MuteKey}", 0) > 0 }
        };

        private readonly Dictionary<AudioParam, float> _groupVolumes = new()
        {
            { AudioParam.Master, PlayerPrefs.GetFloat($"{AudioParam.Master}{VolumeKey}", 1) },
            { AudioParam.BGM, PlayerPrefs.GetFloat($"{AudioParam.BGM}{VolumeKey}", 1) },
            { AudioParam.SFX, PlayerPrefs.GetFloat($"{AudioParam.SFX}{VolumeKey}", 1) },
            { AudioParam.UI, PlayerPrefs.GetFloat($"{AudioParam.UI}{VolumeKey}", 1) }
        };

        public AudioPlayer(
            AudioRig audioRig,
            Dictionary<AudioId, string> names,
            IAudioSpawner spawner,
            ISubscriber<SceneMessage> sceneSubscriber)
        {
            _audioRig = audioRig;
            _names = names;
            _spawner = spawner;
            sceneSubscriber.Subscribe(OnSceneMessage).AddTo(ref DisposableBag);
            Reset();
        }

        public void SetMute(AudioParam param, bool mute)
        {
            _groupMutes[param] = mute;
            PlayerPrefs.SetInt($"{param}{MuteKey}", mute ? 1 : 0);
            SetVolumeInternal(param, mute ? 0 : _groupVolumes[param]);
        }

        public void SetVolume(AudioParam param, float volume)
        {
            _groupVolumes[param] = volume;
            PlayerPrefs.SetFloat($"{param}{VolumeKey}", volume);
            SetVolumeInternal(param, volume);
        }

        public void PlayBGM(AudioId id)
        {
            if (_groupMutes[AudioParam.BGM]) return;

            PlayBgmAsync(id).Forget();
        }

        public void PlaySFX(AudioId id)
        {
            if (_groupMutes[AudioParam.SFX]) return;

            PlaySFXAsync(id).Forget();
        }

        public void PlayUI(AudioId id)
        {
            if (_groupMutes[AudioParam.UI]) return;

            if (_ui != null)
            {
                Return(_ui);
            }

            _ui = Get(_audioRig.UI, id);
            _ui.Play();
        }

        protected override void OnDispose()
        {
            _cts.Cancel();
            _cts.Dispose();
            _cts = null;
            base.OnDispose();
        }

        private async UniTaskVoid PlayBgmAsync(AudioId id)
        {
            if (_bgm != null)
            {
                await FadeAsync(false, AudioParam.BGM);
                Return(_bgm);
            }

            _bgm = Get(_audioRig.BGM, id, loop: true);
            _bgm.Play();
            await FadeAsync(true, AudioParam.BGM);
        }

        private async UniTaskVoid PlaySFXAsync(AudioId id)
        {
            var source = Get(_audioRig.SFX, id);
            _sfx.Add(source);
            source.Play();
            await Awaitable.WaitForSecondsAsync(source.clip.length, _cts.Token);
            Return(source);
            _sfx.Remove(source);
        }

        private AudioSource Get(AudioMixerGroup group, AudioId id, float sourceVolume = 1, bool loop = false) => _spawner.Get(group, _names[id], sourceVolume, loop);

        private void Return(AudioSource source)
        {
            source.Stop();
            _spawner.Return(source);
        }

        private void SetVolumeInternal(AudioParam param, float value)
        {
            // 0 → -80dB
            // 1 → 0dB
            var dB = value <= 0.0001f ? -80f : Mathf.Log10(Mathf.Clamp01(value)) * 20f;
            _audioRig.Mixer.SetFloat(param.AsPrimitive(), dB);
        }

        private async UniTask FadeAsync(
            bool isIn,
            AudioParam param,
            float seconds = 1,
            AnimationCurve curve = null)
        {
            var start = isIn ? 0 : _groupVolumes[param];
            var end = isIn ? _groupVolumes[param] : 0;
            SetVolumeInternal(param, start);

            var time = 0f;
            while (time < seconds)
            {
                if (_cts.Token.IsCancellationRequested) break;

                time += Time.deltaTime;
                var k = Mathf.Clamp01(time / seconds);
                if (curve != null)
                {
                    k = curve.Evaluate(k);
                }

                var cur = Mathf.Lerp(start, end, k);
                SetVolumeInternal(param, cur);
                await Awaitable.NextFrameAsync(_cts.Token);
            }

            SetVolumeInternal(param, end);
        }

        private void Reset()
        {
            _cts.Cancel();
            _cts.Dispose();
            _cts = new CancellationTokenSource();

            _bgm?.Stop();
            _ui?.Stop();
            foreach (var source in _sfx)
            {
                source.Stop();
            }

            SetVolumeInternal(AudioParam.Master, _groupMutes[AudioParam.Master] ? 0 : _groupVolumes[AudioParam.Master]);
            SetVolumeInternal(AudioParam.BGM, _groupMutes[AudioParam.BGM] ? 0 : _groupVolumes[AudioParam.BGM]);
            SetVolumeInternal(AudioParam.SFX, _groupMutes[AudioParam.SFX] ? 0 : _groupVolumes[AudioParam.SFX]);
            SetVolumeInternal(AudioParam.UI, _groupMutes[AudioParam.UI] ? 0 : _groupVolumes[AudioParam.UI]);
        }

        private void OnSceneMessage(SceneMessage message)
        {
            switch (message.Type)
            {
                case SceneMessageType.Unload:
                    Reset();
                    break;
                case SceneMessageType.Load:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}