using Domivium.Client.Core.Battle;
using Domivium.Client.Core.Container;

namespace Domivium.Client.Contents.Battle.Effect
{
    public class AvoidEffect : BattleEffect
    {
        public override BattleEffectId Id => BattleEffectIds.Avoid;
        public AvoidEffect(IUserContainer userUsage, ref BattleEffectContext context) : base(userUsage, ref context) { }

        protected override bool OnActivate()
        {
            this.Log();
            UserContainer.Die(Context.Owner.Position);
            return true;
        }
    }
}