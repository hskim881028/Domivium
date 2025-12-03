using Domivium.Client.Core.Actors;
using Domivium.Client.Core.Battle;
using Domivium.Client.Core.Container;
using Domivium.Client.Data.Stat;

namespace Domivium.Client.Contents.Battle.Effect
{
    public class AttackEffect : BattleEffect
    {
        public override BattleEffectId Id => BattleEffectIds.Attack;

        public AttackEffect(IUserContainer userContainer, ref BattleEffectContext context) : base(userContainer, ref context) { }

        protected override bool OnActivate()
        {
            if (Context.Owner.ActorId == ActorId.Character)
            {
                return UserContainer.Attack();
            }

            if (Context.Owner.ActorId == ActorId.Monster)
            {
                AddGaugeModifier(StatId.ProjectileCapacity, -1, GaugeChannel.Add);
                return true;
            }

            return false;
        }
    }
}