using System.Threading;
using Cysharp.Threading.Tasks;
using Domivium.Client.Core.Actors.Contract;
using UnityEngine;
using UnityEngine.AI;

namespace Domivium.Client.Contents.Actors
{
    public abstract class Unit : Pawn
    {
        private static readonly int FlipDirectionId = Shader.PropertyToID("_FlipDirection");

        [SerializeField] protected NavMeshAgent _agent;

        public override void Initialize(ushort id, Transform parent)
        {
            _agent.updateUpAxis = false;
            _agent.updateRotation = false;

            base.Initialize(id, parent);
        }

        public override UniTask ActivateAsync(CancellationToken token, ActorParam param)
        {
            _agent.updatePosition = true;

            return base.ActivateAsync(token, param);
        }

        public override void Die()
        {
            _agent.updatePosition = false;

            base.Die();
        }

        public override void StartBattle(Vector3 offset, float targetX)
        {
            _agent.isStopped = true;
            _agent.ResetPath();
            _agent.Move(offset);
            _agent.velocity = Vector3.zero;
            SetFlip(targetX);
            base.StartBattle(offset, targetX);
        }

        public override void Battle(float targetX)
        {
            SetFlip(targetX);
            base.Battle(targetX);
        }

        public void SetSpeed(float speed)
        {
            _agent.speed = speed;
        }

        public void SetDestination(Vector3 position)
        {
            _agent.isStopped = false;
            _agent.ResetPath();
            _agent.SetDestination(position);
            SetFlip(position.x);
        }

        private void SetFlip(float targetX)
        {
            if (_renderer.Count <= 0) return;

            var isLeft = transform.position.x < targetX;
            _renderer[0].flipX = isLeft;
            MaterialPropertyBlocks[0].SetFloat(FlipDirectionId, isLeft ? -1 : 1);
            _renderer[0].SetPropertyBlock(MaterialPropertyBlocks[0]);
        }
    }
}