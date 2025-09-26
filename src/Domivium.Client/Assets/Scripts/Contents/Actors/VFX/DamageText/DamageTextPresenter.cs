using System.Threading;
using Cysharp.Threading.Tasks;
using Domivium.Client.Contents.Actors.Contract;
using Domivium.Client.Contents.Actors.Generated;
using Domivium.Client.Core.Actors;
using Domivium.Client.Core.Actors.Contract;
using Domivium.Client.Core.Factory;

namespace Domivium.Client.Contents.Actors
{
    public class DamageTextPresenter : VFXPresenter<DamageText>
    {
        public override ActorId ActorId => ActorIds.DamageText;
        public DamageTextPresenter(DamageText actor, ISystemFactory systemFactory) : base(actor, systemFactory) { }

        public override UniTask ActivateAsync(CancellationToken token, ActorParam param)
        {
            var p = param.As<DamageTextParams>();
            StateSystem.DespawnAsync(p.DespawnTime).Forget();
            return base.ActivateAsync(token, param);
        }
    }
}