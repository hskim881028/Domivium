using System;
using Domivium.Client.Contents.Battle;
using Domivium.Client.Contents.Battle.Effect;
using Domivium.Client.Core.Battle;
using Domivium.Client.Core.Factory;

namespace Domivium.Client.Contents.Factory
{
    public sealed class BattleEffectFactory : IBattleEffectFactory
    {
        public BattleEffect Create(BattleEffectId id, ref BattleEffectContext context)
        {
            if (id == BattleEffectIds.Damage)
            {
                return new DamageEffect(ref context);
            }

            if (id == BattleEffectIds.Heal)
            {
                return new HealEffect(ref context);
            }

            throw new Exception($"Invalid battle effect: {id}");
        }
    }
}