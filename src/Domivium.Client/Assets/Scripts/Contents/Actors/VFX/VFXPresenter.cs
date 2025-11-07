using System.Threading;
using Cysharp.Threading.Tasks;
using Domivium.Client.Contents.Actors.Contract;
using Domivium.Client.Core.Actors;
using Domivium.Client.Core.Actors.Contract;
using Domivium.Client.Core.Factory;

namespace Domivium.Client.Contents.Actors
{
    public abstract class VFXPresenter<TVFX> : ActorPresenter<TVFX>, IVFXPresenter where TVFX : VFX
    {
        protected VFXPresenter(TVFX actor, ISystemFactory systemFactory)
            : base(actor, systemFactory) { }

        public override async UniTask SpawnAsync(CancellationToken token, ActorParam param)
        {
            await base.SpawnAsync(token, param);
            var p = param.As<VFXParams>();
            StateSystem.DespawnAsync(p.DespawnTime).Forget();
        }
    }
}