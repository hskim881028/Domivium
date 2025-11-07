using Domivium.Client.Core.Battle;
using Domivium.Client.Core.Systems;
using Domivium.Client.Data.Stat;

namespace Domivium.Client.Contents.Battle.Ability
{
    public class ChaseAbility : BattleAbility
    {
        private readonly IStageFieldSystem _stageFieldSystem;

        public override BattleAbilityId Id => BattleAbilityIds.Chase;

        public ChaseAbility(IBattleEffectPool effectPool, IStageFieldSystem stageFieldSystem) : base(effectPool)
        {
            _stageFieldSystem = stageFieldSystem;
        }

        protected override bool OnActivate(ref BattleAbilityContext context)
        {
            var source = context.Source;

            var dir = context.Delta - source.Position;
            dir.Normalize();
            source.SetDirection(dir);

            var speed = source.Stat.RateValue(StatId.MoveSpeed);
            var position = source.Position;
            var collider = source.ColliderSize;
            var direction = source.Direction.CurrentValue;
            direction *= speed * context.DeltaTime;
            var nextPosition = _stageFieldSystem.GetNextPosition(position, direction, collider);
            source.SetPosition(nextPosition);
            return true;
        }
    }
}