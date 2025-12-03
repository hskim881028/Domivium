using System;
using Domivium.Client.Contents.Battle;
using Domivium.Client.Contents.Battle.Effect;
using Domivium.Client.Core.Battle;
using Domivium.Client.Core.Container;
using Domivium.Client.Core.Factory;

namespace Domivium.Client.Contents.Factory
{
    public sealed class BattleEffectFactory : IBattleEffectFactory
    {
        private readonly IUserContainer _userUsage;

        public BattleEffectFactory(IUserContainer userUsage)
        {
            _userUsage = userUsage;
        }

        public BattleEffect Create(BattleEffectId id, ref BattleEffectContext context)
        {
            if (id == BattleEffectIds.Attack)
            {
                return new AttackEffect(_userUsage, ref context);
            }

            if (id == BattleEffectIds.Damage)
            {
                return new DamageEffect(_userUsage, ref context);
            }

            if (id == BattleEffectIds.Durability)
            {
                return new DurabilityEffect(_userUsage, ref context);
            }

            if (id == BattleEffectIds.Reload)
            {
                return new ReloadEffect(_userUsage, ref context);
            }

            if (id == BattleEffectIds.Avoid)
            {
                return new AvoidEffect(_userUsage, ref context);
            }


            if (id == BattleEffectIds.Heal)
            {
                return new HealEffect(_userUsage, ref context);
            }

            throw new Exception($"Invalid battle effect: {id}");
        }
    }
}