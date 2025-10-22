using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using Domivium.Client.Contents.Audio.Generated;
using Domivium.Client.Contents.Commands;
using Domivium.Client.Contents.DI;
using Domivium.Client.Contents.ReadModels;
using Domivium.Client.Contents.Services;
using Domivium.Client.Core.Audio;
using Domivium.Client.Core.UI;
using Domivium.Client.Core.UI.Navigation;
using Domivium.Client.Core.UI.Presenter;
using ObservableCollections;
using R3;

namespace Domivium.Client.Contents.UI.Static
{
    public class StageStaticUIPresenter : StaticUIPresenter<StageStaticUIView, IStageStaticUIMessage>, IStageStaticUIMessage
    {
        private readonly SceneService _sceneService;
        private readonly ITowerPlacementCommand _towerPlacementCommand;
        private readonly IStageInventoryCommand _inventoryCommand;
        private readonly IPointerReadModel _pointerRead;
        private readonly IStageInventoryReadModel _inventoryReadModel;
        private readonly ICameraReadModel _cameraReadModel;

        protected override HashSet<UILayer> Layer => UILayer.Set(UILayers.Stage);

        public override UIPriority Priority => UIPriorities.Stage;

        public StageStaticUIPresenter(
            StageStaticUIView view,
            IUINavigation navigation,
            IAudioController audioController,
            SceneService sceneService,
            ITowerPlacementCommand towerPlacementCommand,
            IStageInventoryCommand inventoryCommand,
            IPointerReadModel pointerRead,
            IStageInventoryReadModel inventoryReadModel)
            : base(view, navigation, audioController)
        {
            _sceneService = sceneService;
            _towerPlacementCommand = towerPlacementCommand;
            _inventoryCommand = inventoryCommand;
            _pointerRead = pointerRead;
            _inventoryReadModel = inventoryReadModel;
            _inventoryReadModel.RerollCost.Subscribe(SetRerollCost).AddTo(ref DisposableBag);
            _inventoryReadModel.Soul.Subscribe(SetSoul).AddTo(ref DisposableBag);
            _inventoryReadModel.Tower.Subscribe(SetTower).AddTo(ref DisposableBag);
            _inventoryReadModel.TowerLimit.Subscribe(SetTowerLimit).AddTo(ref DisposableBag);
            _inventoryReadModel.TowerSlot.CollectionChanged += OnChangedInventorySlot;
        }

        protected override void OnDispose()
        {
            _inventoryReadModel.TowerSlot.CollectionChanged -= OnChangedInventorySlot;
            base.OnDispose();
        }

        public void EnterLobby()
        {
            _sceneService.Load(SceneScopeIds.Lobby);
        }

        public void SelectTower(int index)
        {
            if (!_inventoryReadModel.CanPlacementTower(index)) return;

            AudioController.PlayUI(UIAudioId.Click);
            _towerPlacementCommand.Show(index);
            _towerPlacementCommand.Update(_pointerRead.Current);
        }

        public void Reroll()
        {
            _inventoryCommand.Refill(_inventoryReadModel.RerollCost.CurrentValue);
        }

        public void Cancel()
        {
            _towerPlacementCommand.Hide();
        }

        private void SetRerollCost(int rerollCost)
        {
            View.SetRerollCost(rerollCost);
            UpdateView();
        }

        private void SetSoul(int cost)
        {
            View.SetSoul(cost);
            UpdateView();
        }

        private void SetTower(int count)
        {
            View.SetTowerCount(count);
            UpdateView();
        }

        private void SetTowerLimit(int limit)
        {
            View.SetTowerLimit(limit);
            UpdateView();
        }

        private void UpdateView()
        {
            View.SetRerollDimmed(!_inventoryReadModel.CanReroll());

            foreach (var slotIndex in _inventoryReadModel.TowerSlot.Keys)
            {
                var dimmed = !_inventoryReadModel.CanPlacementTower(slotIndex);
                View.SetDimmed(slotIndex, dimmed);
            }
        }

        private void OnChangedInventorySlot(in NotifyCollectionChangedEventArgs<KeyValuePair<int, (int towerId, int cost)>> e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                case NotifyCollectionChangedAction.Replace:
                    View.RefillTower(e.NewItem.Key, e.NewItem.Value.cost);
                    UpdateView();
                    break;
                case NotifyCollectionChangedAction.Remove:
                    View.UserTower(e.OldItem.Key);
                    UpdateView();
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