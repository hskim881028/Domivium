using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Domivium.Client.Contents.Actors.Contract;
using Domivium.Client.Core.Actors;
using Domivium.Client.Core.Actors.Contract;
using Domivium.Client.Core.Component;
using Domivium.Client.Data.Stat;
using UnityEngine;
using UnityEngine.Rendering;

namespace Domivium.Client.Contents.Actors
{
    public abstract class Pawn : Actor
    {
        private static readonly int AlphaId = Shader.PropertyToID("_Alpha");

        [SerializeField] private DebugCircle _debugCircle;
        [SerializeField] private HealthDisplay _healthDisplay;

        [SerializeField] protected List<SpriteRenderer> _renderer;

        private readonly Dictionary<StatId, DebugCircle> _ranges = new();

        protected MaterialPropertyBlock[] MaterialPropertyBlocks;


        public override void Initialize(ushort id, Transform parent)
        {
            MaterialPropertyBlocks = new MaterialPropertyBlock[_renderer.Count];
            for (var i = 0; i < _renderer.Count; i++)
            {
                _renderer[i].shadowCastingMode = ShadowCastingMode.TwoSided;
                var materialPropertyBlock = new MaterialPropertyBlock();
                _renderer[i].GetPropertyBlock(materialPropertyBlock);
                materialPropertyBlock.SetFloat(AlphaId, 1);
                _renderer[i].SetPropertyBlock(materialPropertyBlock);
                MaterialPropertyBlocks[i] = materialPropertyBlock;
                
            }

            base.Initialize(id, parent);
        }

        public override UniTask ActivateAsync(CancellationToken token, ActorParam param)
        {
            SetActiveIndicator(true);

            for (var i = 0; i < _renderer.Count; i++)
            {
                MaterialPropertyBlocks[i].SetFloat(AlphaId, 1);
                _renderer[i].SetPropertyBlock(MaterialPropertyBlocks[i]);
            }

            var p = param.As<UnitParams>();
            transform.localPosition = new Vector3(p.SpawnPoint.x + 0.5f, 0, p.SpawnPoint.y);
            return base.ActivateAsync(token, param);
        }

        public virtual void Die()
        {
            SetActiveIndicator(false);

            for (var i = 0; i < _renderer.Count; i++)
            {
                MaterialPropertyBlocks[i].SetFloat(AlphaId, 0.5f);
                _renderer[i].SetPropertyBlock(MaterialPropertyBlocks[i]);
            }
        }

        public virtual void StartBattle(Vector3 offset, float targetX) { }

        public virtual void Battle(float targetX) { }

        public void SetHealth(int current, int max) => _healthDisplay.Set(current, max);


        public void SetRange(StatId statId, float range, Color color)
        {
            if (!_ranges.ContainsKey(statId))
            {
                var circle = Instantiate(_debugCircle, transform);
                circle.gameObject.layer = gameObject.layer;
                _ranges.Add(statId, circle);
            }

            _ranges[statId].Draw(range, color);
        }

        private void SetActiveIndicator(bool isActive)
        {
            _healthDisplay.gameObject.SetActive(isActive);
            foreach (var circle in _ranges.Values)
            {
                circle.gameObject.SetActive(isActive);
            }
        }
    }
}