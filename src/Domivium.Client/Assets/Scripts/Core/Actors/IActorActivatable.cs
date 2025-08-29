using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Domivium.Client.Core.Actors.Contract;
using UnityEngine;

namespace Domivium.Client.Core.Actors
{
    public interface IActorActivatable
    {
        public void Initialize(Transform parent, Action onDespawn);
        public UniTask ShowAsync(CancellationToken token, ActorParam param, bool immediately = false);
        public UniTask HideAsync(CancellationToken token, bool immediately = false);
    }
}