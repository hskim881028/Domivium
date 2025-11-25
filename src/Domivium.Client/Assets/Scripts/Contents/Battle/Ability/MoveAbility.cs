using Domivium.Client.Core.Battle;
using Domivium.Client.Core.Systems;
using Domivium.Client.Data.Stat;

namespace Domivium.Client.Contents.Battle.Ability
{
    public class MoveAbility : BattleAbility
    {
        private readonly IStageFieldSystem _stageFieldSystem;

        public override BattleAbilityId Id => BattleAbilityIds.Move;
        public override BattleCueId CueId => BattleCueIds.Move;

        public MoveAbility(IBattleEffectPool effectPool, IStageFieldSystem stageFieldSystem) : base(effectPool)
        {
            _stageFieldSystem = stageFieldSystem;
        }

        public override float Activate(ref BattleAbilityContext context)
        {
            var source = context.Source;
            var speed = source.Stat.RateValue(StatId.MoveSpeed);
            var position = source.Position;
            var collider = source.ColliderSize;
            var direction = source.Direction.CurrentValue;
            direction *= speed * context.DeltaTime;
            var nextPosition = _stageFieldSystem.GetNextPosition(position, direction, collider);
            source.SetPosition(nextPosition);
            return 0;
        }
    }
}