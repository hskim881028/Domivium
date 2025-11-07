using Domivium.Client.Core.Factory;

namespace Domivium.Client.Contents.Actors
{
    public class MonsterPresenter : UnitPresenter<Monster>
    {
        public MonsterPresenter(
            Monster actor,
            ISystemFactory systemFactory)
            : base(actor, systemFactory) { }

        protected override void OnHealthGaugeChanged()
        {
            base.OnHealthGaugeChanged();
            
            Actor.ShowAsync().Forget();
        }
    }
}