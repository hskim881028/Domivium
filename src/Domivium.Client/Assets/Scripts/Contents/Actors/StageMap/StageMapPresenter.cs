using System.Threading;
using Cysharp.Threading.Tasks;
using Domivium.Client.Core.Actors;
using Domivium.Client.Core.Actors.Contract;
using UnityEngine;

namespace Domivium.Client.Contents.Actors
{
    public sealed class StageMapPresenter : ActorPresenter<StageMap>
    {
        public StageMapPresenter(StageMap actor) : base(actor) { }

        public override async UniTask ShowAsync(CancellationToken token, ActorParam param, bool immediately = false)
        {
            await base.ShowAsync(token, param, immediately);

            await Awaitable.WaitForSecondsAsync(3, token);
            HideAsync(token).Forget();
        }
    }
}