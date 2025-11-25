using System;
using Domivium.Client.Contents.Battle;
using Domivium.Client.Contents.Battle.Effect;
using Domivium.Client.Core.Battle;
using Domivium.Client.Core.Factory;
using Domivium.Client.Core.Systems;

namespace Domivium.Client.Contents.Factory
{
    public sealed class BattleEffectFactory : IBattleEffectFactory
    {
        private readonly IItemUsageSystemCommand _itemUsage;

        public BattleEffectFactory(IItemUsageSystemCommand itemUsage)
        {
            _itemUsage = itemUsage;
        }

        public BattleEffect Create(BattleEffectId id, ref BattleEffectContext context)
        {
            if (id == BattleEffectIds.Attack)
            {
                return new AttackEffect(_itemUsage, ref context);
            }

            if (id == BattleEffectIds.Damage)
            {
                return new DamageEffect(_itemUsage, ref context);
            }

            if (id == BattleEffectIds.Durability)
            {
                return new DurabilityEffect(_itemUsage, ref context);
            }

            if (id == BattleEffectIds.Reload)
            {
                return new ReloadEffect(_itemUsage, ref context);
            }

            if (id == BattleEffectIds.Avoid)
            {
                return new AvoidEffect(_itemUsage, ref context);
            }


            if (id == BattleEffectIds.Heal)
            {
                return new HealEffect(_itemUsage, ref context);
            }

            throw new Exception($"Invalid battle effect: {id}");
        }
    }
}