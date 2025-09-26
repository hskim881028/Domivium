using System.Threading;
using Cysharp.Threading.Tasks;
using Domivium.Client.Contents.Actors.Contract;
using Domivium.Client.Core.Actors;
using Domivium.Client.Core.Actors.Contract;
using Domivium.Client.Core.Factory;

namespace Domivium.Client.Contents.Actors
{
    public abstract class CampPresenter<TCamp> : ActorPresenter<TCamp>, ICampPresenter where TCamp : Camp
    {
        public int Index { get; private set; }

        protected CampPresenter(TCamp actor, ISystemFactory systemFactory) : base(actor, systemFactory) { }

        public override UniTask ActivateAsync(CancellationToken token, ActorParam param)
        {
            var p = param.As<CampParams>();
            Index = p.Index;

            return base.ActivateAsync(token, param);
        }
    }
}