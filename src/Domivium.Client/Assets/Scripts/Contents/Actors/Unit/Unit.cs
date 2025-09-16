using Domivium.Client.Core.Actors;
using Domivium.Client.Core.Component;
using Domivium.Client.Core.Utility;
using UnityEngine;

namespace Domivium.Client.Contents.Actors
{
    public abstract class Unit : Actor, IUnit
    {
        [SerializeField] private HealthDisplay _healthDisplay;
        [SerializeField] private DebugCircle _attackRangeCircle;
        [SerializeField] private DebugCircle _detectionRangeCircle;

        public virtual void Tick(float deltaTime) { }

        public void SetHealth(int current, int max) => _healthDisplay.Set(current, max);

        public void SetAttackRange(int attackRange)
        {
            var radius = attackRange * Constant.Percent;
            _attackRangeCircle.Draw(radius, Color.crimson);
        }

        public void SetDetectionRange(float detectionRange)
        {
            var radius = detectionRange * Constant.Percent;
            this.Log(radius);
            _detectionRangeCircle.Draw(radius, Color.chartreuse);
        }
    }
}