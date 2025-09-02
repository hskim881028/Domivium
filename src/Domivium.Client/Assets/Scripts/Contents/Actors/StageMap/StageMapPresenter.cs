using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using Domivium.Client.Contents.Context;
using Domivium.Client.Contents.ReadModels;
using Domivium.Client.Core.Actors;
using Domivium.Client.Core.Context;
using ObservableCollections;
using R3;
using UnityEngine;

namespace Domivium.Client.Contents.Actors.StageMap
{
    public sealed class StageMapPresenter : ActorPresenter<StageMap>
    {
        private readonly ITowerPlacementReadModel _read;

        public StageMapPresenter(
            StageMap actor,
            StageContext stageContext,
            ITowerPlacementReadModel read)
            : base(actor)
        {
            stageContext.Mode.Subscribe(OnChangeMode).AddTo(ref Disposable);

            _read = read;
            _read.StagedTower.CollectionChanged += OnChangedStagedTower;
            _read.PreviewTower.CollectionChanged += OnChangedPreviewTower;
        }

        protected override void OnDispose()
        {
            _read.StagedTower.CollectionChanged -= OnChangedStagedTower;
            _read.PreviewTower.CollectionChanged -= OnChangedPreviewTower;
        }

        private void OnChangeMode(StageMode mode)
        {
            Actor.SetActivePreviewGrid(mode == StageModes.TowerPlacement);
        }

        private void OnChangedStagedTower(in NotifyCollectionChangedEventArgs<Vector3Int> e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    Actor.Placement(e.NewItem);
                    foreach (var item in e.NewItems)
                    {
                        Actor.Placement(item);
                    }
                    break;
                case NotifyCollectionChangedAction.Move:
                case NotifyCollectionChangedAction.Remove:
                case NotifyCollectionChangedAction.Replace:
                case NotifyCollectionChangedAction.Reset:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void OnChangedPreviewTower(in NotifyCollectionChangedEventArgs<KeyValuePair<Vector3Int, bool>> e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                case NotifyCollectionChangedAction.Replace:
                    Actor.DrawPreview(e.NewItem.Key, e.NewItem.Value);
                    foreach (var item in e.NewItems)
                    {
                        Actor.DrawPreview(item.Key, item.Value);
                    }
                    break;
                case NotifyCollectionChangedAction.Remove:
                case NotifyCollectionChangedAction.Reset:
                    Actor.ResetPreview();
                    break;
                case NotifyCollectionChangedAction.Move:
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}