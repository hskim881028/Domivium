using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Domivium.Client.Contents.Actors.Contract;
using Domivium.Client.Core.Actors;
using Domivium.Client.Core.Actors.Contract;
using Domivium.Client.Core.Message;
using Domivium.Client.Core.Utility;
using MessagePipe;
using UnityEngine;

namespace Domivium.Client.Contents.Actors
{
    public class DamageTextPresenter : VFXPresenter<DamageText>
    {
        public DamageTextPresenter(Guid id, DamageText actor, IPublisher<ActorTagMessage> tagPublisher)
            : base(id, actor, tagPublisher) { }

        public override UniTask ActivateAsync(CancellationToken token, ActorParam param)
        {
            var p = param.As<DamageTextParams>();
            DespawnAsync(p.DespawnTime, token).Forget();
            return base.ActivateAsync(token, param);
        }

        private async UniTaskVoid DespawnAsync(float despawnTime, CancellationToken token)
        {
            await Awaitable.WaitForSecondsAsync(despawnTime, token);
            TagSet.Add(ActorTag.Despawn);
        }
    }
}