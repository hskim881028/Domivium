using System.Threading;
using Cysharp.Threading.Tasks;
using Domivium.Client.Contents.Actors.Contract;
using Domivium.Client.Core.Actors;
using Domivium.Client.Core.Actors.Contract;
using Unity.AI.Navigation;
using UnityEngine;

namespace Domivium.Client.Contents.Actors
{
    public abstract class Camp : Actor
    {
        private NavMeshModifierVolume _volume;

        protected override void OnAwake()
        {
            _volume = GetComponentInChildren<NavMeshModifierVolume>();
            base.OnAwake();
        }

        public override UniTask ActivateAsync(CancellationToken token, ActorParam param)
        {
            var p = param.As<CampParams>();
            transform.localPosition = new Vector3(p.SpawnPoint.x + 0.5f, 0, p.SpawnPoint.y);
            return base.ActivateAsync(token, param);
        }
    }
}