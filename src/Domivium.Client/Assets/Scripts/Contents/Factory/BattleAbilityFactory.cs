using System;
using Domivium.Client.Contents.Battle;
using Domivium.Client.Contents.Battle.Ability;
using Domivium.Client.Core.Battle;
using Domivium.Client.Core.Factory;

namespace Domivium.Client.Contents.Factory
{
    public sealed class BattleAbilityFactory : IBattleAbilityFactory
    {
        private readonly IBattleEffectPool _effectPool;

        public BattleAbilityFactory(IBattleEffectPool effectPool)
        {
            _effectPool = effectPool;
        }

        public BattleAbility Create(BattleAbilityId id)
        {
            if (id == BattleAbilityIds.Attack)
            {
                return new AttackAbility(_effectPool);
            }

            if (id == BattleAbilityIds.Heal)
            {
                return new HealAbility(_effectPool);
            }

            throw new Exception($"Invalid battle ability: {id}");
        }
    }
}