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
            SetFlip(-1);
            return base.ActivateAsync(token, param);
        }

        public override void Tick(float deltaTime)
        {
            if (_agent.isStopped) return;

            if (Mathf.Approximately(_agent.velocity.x, 0)) return;

            SetFlip(_agent.velocity.x > 0 ? 1 : -1);
            base.Tick(deltaTime);
        }

        public override void Die()
        {
            _agent.updatePosition = false;
            base.Die();
        }

        public override void StartBattle(Vector3 offset, Vector3 target)
        {
            _agent.isStopped = true;
            _agent.ResetPath();
            _agent.Move(offset);
            _agent.velocity = Vector3.zero;
            SetFlip(transform.position.x < target.x ? 1 : -1);
            base.StartBattle(offset, target);
        }

        public override void Battle(Vector3 target)
        {
            SetFlip(transform.position.x < target.x ? 1 : -1);
            base.Battle(target);
        }

        public void SetSpeed(float speed)
        {
            _agent.speed = speed;
        }

        public void SetDestination(Vector3 target)
        {
            _agent.isStopped = false;
            _agent.ResetPath();
            _agent.SetDestination(target);
            SetFlip(transform.position.x < target.x ? 1 : -1);
        }

        private void SetFlip(int direction)
        {
            // true -> right
            _renderer[0].flipX = direction > 0;
            MaterialPropertyBlocks[0].SetFloat(FlipDirectionId, direction);
            _renderer[0].SetPropertyBlock(MaterialPropertyBlocks[0]);
        }
    }
}