using System.Threading;
using Cysharp.Threading.Tasks;
using Domivium.Client.Contents.Actors.Contract;
using Domivium.Client.Contents.Actors.Generated;
using Domivium.Client.Contents.ReadModels;
using Domivium.Client.Core.Actors;
using Domivium.Client.Core.Actors.Contract;
using Domivium.Client.Core.Factory;
using UnityEngine;

namespace Domivium.Client.Contents.Actors
{
    public class NexusPresenter : UnitPresenter<Nexus>
    {
        public override ActorId ActorId => ActorIds.Nexus;
        public Vector3Int Cell { get; private set; }

        public NexusPresenter(Nexus actor, ISystemFactory systemFactory, IBattleService battleService)
            : base(actor, systemFactory, battleService) { }

        public override UniTask ActivateAsync(CancellationToken token, ActorParam param)
        {
            var p = param.As<UnitParams>();
            Cell = p.SpawnPoint;
            return base.ActivateAsync(token, param);
        }
    }
}