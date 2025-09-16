using Domivium.Client.Contents.ReadModels;
using Domivium.Client.Data.Stat;
using R3;

namespace Domivium.Client.Contents.Actors
{
    public class CharacterPresenter : UnitPresenter<Character>
    {
        public CharacterPresenter(
            Character actor,
            IBattleReadModel read)
            : base(actor)
        {
            read.TargetPosition.Subscribe(actor.SetTargetPosition).AddTo(ref Disposable);
        }

        protected override void OnSpeedStatChanged()
        {
            var speed = BattleSystem.Stat.Value(StatId.Speed);
            Actor.SetSpeed(speed);
            base.OnSpeedStatChanged();
        }

        protected override void OnDispose() { }
    }
}