using Domivium.Client.Core.Battle;
using Domivium.Client.Core.Systems;

namespace Domivium.Client.Contents.Battle.Ability
{
    public class AvoidAbility : BattleAbility
    {
        private const float Multiply = 1f;

        private readonly IStageFieldSystem _stageFieldSystem;

        public override BattleAbilityId Id => BattleAbilityIds.Avoid;
        public override BattleCueId CueId => BattleCueIds.Avoid;

        public AvoidAbility(IBattleEffectPool effectPool, IStageFieldSystem stageFieldSystem) : base(effectPool)
        {
            _stageFieldSystem = stageFieldSystem;
        }

        protected override bool OnActivate(ref BattleAbilityContext context)
        {
            var source = context.Source;
            var position = source.Position;
            var collider = source.ColliderSize;
            var direction = source.Direction.CurrentValue;
            direction *= Multiply;
            var nextPosition = _stageFieldSystem.GetNextPosition(position, direction, collider);
            context.Source.SetPosition(nextPosition);
            return true;
        }
    }
}