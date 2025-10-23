using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using Domivium.Client.Contents.Actors.Generated;
using Domivium.Client.Contents.Context;
using Domivium.Client.Contents.ReadModels;
using Domivium.Client.Core.Actors;
using Domivium.Client.Core.Battle;
using Domivium.Client.Core.Context;
using Domivium.Client.Core.Factory;
using Domivium.Client.Data.Info;
using ObservableCollections;
using R3;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Domivium.Client.Contents.Actors
{
    public sealed class StageFieldPresenter : ActorPresenter<StageField>
    {
        private readonly IStageInventoryReadModel _inventoryReadModel;
        public Tilemap Grid => Actor.Background;
        public override ActorId ActorId => ActorIds.Map;

        public StageFieldPresenter(
            StageField actor,
            ISystemFactory systemFactory,
            StageContext stageContext,
            IStageInventoryReadModel inventoryReadModel,
            ITowerPlacementReadModel towerPlacementReadModel)
            : base(actor, systemFactory)
        {
            _inventoryReadModel = inventoryReadModel;
            _inventoryReadModel.Towers.CollectionChanged += OnChangedTowers;
            _inventoryReadModel.Barrier.CollectionChanged += OnChangedBarrier;

            stageContext.Mode.Subscribe(OnChangeMode).AddTo(ref DisposableBag);
            towerPlacementReadModel.Ready.Subscribe(OnReady).AddTo(ref DisposableBag);
            towerPlacementReadModel.PreviewTower.Subscribe(OnPreviewTower).AddTo(ref DisposableBag);
        }

        protected override void OnDispose()
        {
            _inventoryReadModel.Towers.CollectionChanged -= OnChangedTowers;
            _inventoryReadModel.Barrier.CollectionChanged -= OnChangedBarrier;
            base.OnDispose();
        }

        private void OnReady(bool ready)
        {
            if (!ready) return;

            Actor.BuildNavMesh();
        }

        private void OnPreviewTower(StageCellInfo cellInfo)
        {
            Actor.DrawPreview(cellInfo.Cell, cellInfo.Tag);
        }

        private void OnChangeMode(StageMode mode)
        {
            Actor.SetActivePreviewGrid(mode == StageModes.TowerPlacement);
        }

        private void OnChangedTowers(in NotifyCollectionChangedEventArgs<KeyValuePair<Vector3Int, IBattleSystem>> e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                case NotifyCollectionChangedAction.Replace:
                    Actor.Placement(e.NewItem.Key);
                    break;
                case NotifyCollectionChangedAction.Remove:
                    Actor.Release(e.OldItem.Key);
                    break;
                case NotifyCollectionChangedAction.Move:
                case NotifyCollectionChangedAction.Reset:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void OnChangedBarrier(in NotifyCollectionChangedEventArgs<Vector3Int> e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                case NotifyCollectionChangedAction.Replace:
                    Actor.Placement(e.NewItem);
                    break;
                case NotifyCollectionChangedAction.Remove:
                    Actor.Release(e.OldItem);
                    break;
                case NotifyCollectionChangedAction.Move:
                case NotifyCollectionChangedAction.Reset:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}