using System.Threading;
using Cysharp.Threading.Tasks;
using Domivium.Client.Contents.Actors.Contract;
using Domivium.Client.Contents.State;
using Domivium.Client.Core.Actors;
using Domivium.Client.Core.Actors.Contract;
using Domivium.Client.Core.Factory;
using UnityEngine;

namespace Domivium.Client.Contents.Actors
{
    public class TowerPresenter : UnitPresenter<Tower>, ICellOccupant
    {
        public Vector3Int Cell { get; private set; }

        public TowerPresenter(Tower actor, ISystemFactory systemFactory, IActorFinder actorFinder)
            : base(actor, systemFactory, actorFinder) { }

        public override UniTask ActivateAsync(CancellationToken token, ActorParam param)
        {
            var p = param.As<UnitParams>();
            Cell = p.SpawnPoint;
            return base.ActivateAsync(token, param);
        }

        protected override void OnIdleTick()
        {
            if (!ActorFinder.FindNearestBattleTarget(TargetActionId, BattleSystem, out var target)) return;

            Target = target;
            StateSystem.TryTransit(StateTags.Battle);
            base.OnIdleTick();
        }
    }
}