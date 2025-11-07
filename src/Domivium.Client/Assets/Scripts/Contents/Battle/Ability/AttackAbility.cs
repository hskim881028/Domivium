using Cysharp.Threading.Tasks;
using Domivium.Client.Contents.Actors.Generated;
using Domivium.Client.Core.Actors;
using Domivium.Client.Core.Battle;
using Domivium.Client.Core.Factory;
using Domivium.Client.Data.Stat;

namespace Domivium.Client.Contents.Battle.Ability
{
    public class AttackAbility : BattleAbility
    {
        private readonly IBattleAbilityFactory _abilityFactory;
        private readonly IActorSpawner _actorSpawner;
        private readonly IActorParamFactory _actorParamFactory;

        public override BattleAbilityId Id => BattleAbilityIds.Attack;
        public override BattleCueId CueId => BattleCueIds.Attack;

        public AttackAbility(
            IBattleEffectPool effectPool,
            IBattleAbilityFactory abilityFactory,
            IActorParamFactory actorParamFactory,
            IActorSpawner actorSpawner) : base(effectPool)
        {
            _abilityFactory = abilityFactory;
            _actorSpawner = actorSpawner;
            _actorParamFactory = actorParamFactory;
        }

        public override bool CanActivate(IBattleSystem source)
        {
            if (!base.CanActivate(source)) return false;

            var cur = source.Gauge.Current(StatId.ProjectileCapacity);
            return cur > 0;
        }

        protected override bool OnActivate(ref BattleAbilityContext context)
        {
            var effect = EffectPool.Get(BattleEffectIds.Attack, context.Source, context.Source);
            context.Source.ActivateEffect(effect);
            var target = context.Source.ActorId == ActorIds.Character ? ActorIds.Monster : ActorIds.Character;
            var stat = context.Source.Stat;
            var position = context.Source.MuzzlePosition;
            var direction = context.Source.LookAt.CurrentValue;
            var abilities = _abilityFactory.GetAbilities(ActorIds.Projectile);
            var param = _actorParamFactory.CreateProjectile(1, target, stat, position, direction, abilities);
            _actorSpawner.SpawnAsync(ActorIds.Projectile, param).Forget();
            return true;
        }
    }
}