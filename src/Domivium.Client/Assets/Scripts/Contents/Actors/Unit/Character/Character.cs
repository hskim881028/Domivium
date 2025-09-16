using System;
using UnityEngine;
using UnityEngine.AI;

namespace Domivium.Client.Contents.Actors
{
    public class Character : Unit
    {
        [SerializeField] private NavMeshAgent _agent;

        public override void Initialize(Transform parent, Action onDespawn)
        {
            _agent.updateUpAxis = false;
            _agent.updateRotation = false;
            base.Initialize(parent, onDespawn);
        }

        public void SetSpeed(int speed)
        {
            _agent.speed = speed * Constant.Percent;
        }

        public void SetTargetPosition(Vector3 position)
        {
            _agent.SetDestination(position);
        }
    }
}