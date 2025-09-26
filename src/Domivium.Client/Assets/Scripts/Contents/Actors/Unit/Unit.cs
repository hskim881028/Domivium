using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Domivium.Client.Contents.Actors.Contract;
using Domivium.Client.Core.Actors;
using Domivium.Client.Core.Actors.Contract;
using Domivium.Client.Core.Component;
using UnityEngine;
using UnityEngine.AI;

namespace Domivium.Client.Contents.Actors
{
    public abstract class Unit : Actor
    {
        private static readonly int AlphaId = Shader.PropertyToID("_Alpha");

        [SerializeField] private HealthDisplay _healthDisplay;
        [SerializeField] private DebugCircle _attackRangeCircle;
        [SerializeField] private DebugCircle _detectionRangeCircle;
        [SerializeField] protected NavMeshAgent _agent;
        [SerializeField] private List<MeshRenderer> _meshRenderer;

        private MaterialPropertyBlock[] _materialPropertyBlocks;

        public override void Initialize(ushort id, Transform parent)
        {
            _materialPropertyBlocks = new MaterialPropertyBlock[_meshRenderer.Count];
            for (var i = 0; i < _meshRenderer.Count; i++)
            {
                var materialPropertyBlock = new MaterialPropertyBlock();
                _meshRenderer[i].GetPropertyBlock(materialPropertyBlock);
                materialPropertyBlock.SetFloat(AlphaId, 1);
                _meshRenderer[i].SetPropertyBlock(materialPropertyBlock);
                _materialPropertyBlocks[i] = materialPropertyBlock;
            }

            if (_agent != null)
            {
                _agent.updateUpAxis = false;
                _agent.updateRotation = false;
            }

            base.Initialize(id, parent);
        }

        public override UniTask ActivateAsync(CancellationToken token, ActorParam param)
        {
            SetActiveIndicator(true);
            if (_agent != null)
            {
                _agent.updatePosition = true;
            }

            for (var i = 0; i < _meshRenderer.Count; i++)
            {
                _materialPropertyBlocks[i].SetFloat(AlphaId, 1);
                _meshRenderer[i].SetPropertyBlock(_materialPropertyBlocks[i]);
            }

            var p = param.As<UnitParams>();
            transform.localPosition = new Vector3(p.SpawnPoint.x + 0.5f, 0, p.SpawnPoint.y);
            return base.ActivateAsync(token, param);
        }

        public override void Deactivate()
        {
            base.Deactivate();
        }

        public void Die()
        {
            SetActiveIndicator(false);

            if (_agent != null)
            {
                _agent.updatePosition = false;
            }

            for (var i = 0; i < _meshRenderer.Count; i++)
            {
                _materialPropertyBlocks[i].SetFloat(AlphaId, 0.5f);
                _meshRenderer[i].SetPropertyBlock(_materialPropertyBlocks[i]);
            }
        }

        private NavMeshPath _path;

        protected override void OnAwake()
        {
            _path = new NavMeshPath();
            base.OnAwake();
        }

        public void Battle(Vector3 offset)
        {
            _agent.isStopped = true;
            _agent.ResetPath();
            _agent.Move(offset);
            _agent.velocity = Vector3.zero;
        }

        public void SetHealth(int current, int max) => _healthDisplay.Set(current, max);

        public void SetSpeed(float speed)
        {
            if (_agent == null) return;

            _agent.speed = speed;
        }

        public void SetDestination(Vector3 position)
        {
            if (_agent == null) return;

            _agent.isStopped = false;
            _agent.ResetPath();
            _agent.SetDestination(position);
        }

        public void SetAttackRange(float range)
        {
            _attackRangeCircle.Draw(range, Color.crimson);
        }

        public void SetDetectionRange(float range)
        {
            _detectionRangeCircle.Draw(range, Color.chartreuse);
        }

        public void SetHitRange(float range) { }

        private void SetActiveIndicator(bool isActive)
        {
            _healthDisplay.gameObject.SetActive(isActive);
            _attackRangeCircle.gameObject.SetActive(isActive);
            _detectionRangeCircle.gameObject.SetActive(isActive);
        }
    }
}