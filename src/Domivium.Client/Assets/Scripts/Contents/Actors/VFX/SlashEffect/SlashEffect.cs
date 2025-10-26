using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Domivium.Client.Contents.Actors.Contract;
using Domivium.Client.Contents.Actors.Generated;
using Domivium.Client.Core.Actors;
using Domivium.Client.Core.Actors.Contract;
using UnityEngine;

namespace Domivium.Client.Contents.Actors
{
    public class SlashEffect : VFX
    {
        private static readonly int ColorId = Shader.PropertyToID("_EdgeCol");

        [SerializeField] private List<ParticleSystem> _particleSystem;

        [SerializeField] [ColorUsage(true, true)]
        private Color _allyEdgeColor;

        [SerializeField] private List<Color> _allyColors;

        [SerializeField] [ColorUsage(true, true)]
        private Color _monsterEdgeColor;

        [SerializeField] private List<Color> _monsterColors;

        private Renderer[] _renderers;
        private MaterialPropertyBlock[] _materialPropertyBlocks;

        private readonly Dictionary<ActorId, List<Color>> _colors = new();
        private readonly Dictionary<ActorId, Color> _edgeColors = new();

        protected override void OnAwake()
        {
            base.OnAwake();
            _colors.Add(ActorIds.Character, _allyColors);
            _colors.Add(ActorIds.Tower, _allyColors);
            _colors.Add(ActorIds.Monster, _monsterColors);

            _edgeColors.Add(ActorIds.Character, _allyEdgeColor);
            _edgeColors.Add(ActorIds.Tower, _allyEdgeColor);
            _edgeColors.Add(ActorIds.Monster, _monsterEdgeColor);
        }

        public override void Initialize(ushort uid, Transform parent)
        {
            _renderers = new Renderer[_particleSystem.Count];
            _materialPropertyBlocks = new MaterialPropertyBlock[_particleSystem.Count];
            for (var i = 0; i < _particleSystem.Count; i++)
            {
                var mpb = new MaterialPropertyBlock();
                _renderers[i] = _particleSystem[i].GetComponent<Renderer>();
                _renderers[i].GetPropertyBlock(mpb);
                _materialPropertyBlocks[i] = mpb;
            }

            base.Initialize(uid, parent);
        }

        public override UniTask ActivateAsync(CancellationToken token, ActorParam param)
        {
            var p = param.As<SlashEffectParams>();
            transform.localPosition = p.Position;
            transform.localScale = new Vector3(p.Direction, 1, 1);

            if (!_colors.TryGetValue(p.ActorId, out var colors) ||
                !_edgeColors.TryGetValue(p.ActorId, out var edgeColor))
            {
                return base.ActivateAsync(token, param);
            }

            for (var i = 0; i < _particleSystem.Count; i++)
            {
                _materialPropertyBlocks[i].SetColor(ColorId, edgeColor);
                _renderers[i].SetPropertyBlock(_materialPropertyBlocks[i]);
                var main = _particleSystem[i].main;
                main.startColor = colors[i];
                _particleSystem[i].Play();
            }
            return base.ActivateAsync(token, param);
        }
    }
}