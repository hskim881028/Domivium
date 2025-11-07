using Domivium.Client.Core.Battle;
using Domivium.Client.Core.Systems;
using Domivium.Client.Data.Stat;

namespace Domivium.Client.Contents.Battle.Ability
{
    public class MoveAbility : BattleAbility
    {
        private readonly IStageSystem _stageSystem;
        public override BattleAbilityId Id => BattleAbilityIds.Move;
        public override BattleCueId CueId => BattleCueIds.Move;

        public MoveAbility(IBattleEffectPool effectPool, IStageSystem stageSystem) : base(effectPool)
        {
            _stageSystem = stageSystem;
        }

        protected override bool OnActivate(ref BattleAbilityContext context)
        {
            var source = context.Source;
            var speed = source.Stat.RateValue(StatId.MoveSpeed);
            var position = source.Position;
            var collider = source.ColliderSize;
            var direction = source.Direction.CurrentValue;
            direction *= speed * context.DeltaTime;
            var nextPosition = _stageSystem.GetNextPosition(position, direction, collider);
            source.SetPosition(nextPosition);
            return true;
        }
    }
}