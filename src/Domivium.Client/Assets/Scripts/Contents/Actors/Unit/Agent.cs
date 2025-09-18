using Domivium.Client.Core.Actors;
using UnityEngine;
using UnityEngine.AI;

namespace Domivium.Client.Contents.Actors
{
    public class Agent : Unit, IAgent
    {
        [SerializeField] private NavMeshAgent _agent;

        public override void Initialize(Transform parent)
        {
            _agent.updateUpAxis = false;
            _agent.updateRotation = false;
            base.Initialize(parent);
        }

        public void SetSpeed(int speed)
        {
            _agent.speed = speed * Constant.Percent;
        }

        public void SetTargetPosition(Vector3 position)
        {
            _agent.SetDestination(position);
        }

        public override void Tick(float deltaTime)
        {
            base.Tick(deltaTime);
        }
    }
}