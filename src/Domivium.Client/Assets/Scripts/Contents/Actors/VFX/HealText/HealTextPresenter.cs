using System.Threading;
using Cysharp.Threading.Tasks;
using Domivium.Client.Contents.Actors.Contract;
using Domivium.Client.Contents.Actors.Generated;
using Domivium.Client.Core.Actors;
using Domivium.Client.Core.Actors.Contract;
using Domivium.Client.Core.Factory;

namespace Domivium.Client.Contents.Actors
{
    public class HealTextPresenter : VFXPresenter<HealText>
    {
        public override ActorId ActorId => ActorIds.HealText;
        public HealTextPresenter(HealText actor, ISystemFactory systemFactory) : base(actor, systemFactory) { }

        public override UniTask ActivateAsync(CancellationToken token, ActorParam param)
        {
            var p = param.As<HealTextParams>();
            StateSystem.DespawnAsync(p.DespawnTime).Forget();
            return base.ActivateAsync(token, param);
        }
    }
}