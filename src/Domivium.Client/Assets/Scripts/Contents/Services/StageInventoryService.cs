using Domivium.Client.Contents.Actors.Generated;
using Domivium.Client.Contents.Battle;
using Domivium.Client.Contents.Commands;
using Domivium.Client.Contents.ReadModels;
using Domivium.Client.Contents.State;
using Domivium.Client.Core.Battle;
using Domivium.Client.Core.Message;
using MessagePipe;
using ObservableCollections;
using R3;

namespace Domivium.Client.Contents.Services
{
    public sealed class StageInventoryService : Disposable, IStageInventoryReadModel, IStageInventoryCommand
    {
        private readonly MasterDbService _masterDbService;
        private readonly IBattleService _battleService;
        private readonly IPublisher<BattleCueMessage> _cuePublisher;
        private readonly ReactiveProperty<int> _rerollCost = new();
        private readonly ReactiveProperty<int> _soul = new();
        private readonly ReactiveProperty<int> _tower = new();
        private readonly ReactiveProperty<int> _towerLimit = new();
        private readonly ObservableDictionary<int, (int towerId, int cost)> _towerSlot = new();

        public ReadOnlyReactiveProperty<int> RerollCost => _rerollCost;
        public ReadOnlyReactiveProperty<int> Soul => _soul;
        public ReadOnlyReactiveProperty<int> Tower => _tower;
        public ReadOnlyReactiveProperty<int> TowerLimit => _towerLimit;
        public IReadOnlyObservableDictionary<int, (int towerId, int cost)> TowerSlot => _towerSlot;

        public StageInventoryService(
            MasterDbService masterDbService,
            IBattleService battleService,
            IPublisher<BattleCueMessage> cuePublisher,
            ISubscriber<ActorStateMessage> actorStateSubscriber)
        {
            _masterDbService = masterDbService;
            _battleService = battleService;
            _cuePublisher = cuePublisher;
            actorStateSubscriber.Subscribe(OnActorStateMessage).AddTo(ref DisposableBag);
        }

        public bool CanReroll() => _soul.CurrentValue >= _rerollCost.CurrentValue;

        public bool CanPlacementTower(int slotIndex)
        {
            if (_tower.CurrentValue >= _towerLimit.CurrentValue) return false;

            if (!_towerSlot.TryGetValue(slotIndex, out var tuple)) return false;

            return tuple.cost <= _soul.Value;
        }

        public void Initialize(int rerollCost, int towerLimit)
        {
            _rerollCost.Value = rerollCost;
            _towerLimit.Value = towerLimit;
            Refill(0);
        }

        public void Use(int slotIndex)
        {
            _towerSlot.TryGetValue(slotIndex, out var value);
            _soul.Value -= value.cost;
            _tower.Value += 1;
            _towerSlot.Remove(slotIndex);
        }

        public void Refill(int cost)
        {
            _soul.Value -= cost;
            for (var i = 0; i < 5; i++)
            {
                var row = _masterDbService.DB.TowerRowTable.FindById(1);
                _towerSlot[i] = (row.Id, row.Cost);
            }
        }

        private void OnActorStateMessage(ActorStateMessage message)
        {
            if (message.ActorId == ActorIds.Monster && message.Tag == StateTags.Die)
            {
                if (!_battleService.FindTarget(ActorIds.Monster, message.Uid, out var monster)) return;

                if (!_masterDbService.DB.MonsterRowTable.TryFindById(message.Id, out var row)) return;

                this.Log($"rarity : {row.Soul}");
                _soul.Value += row.Soul;
                var nexus = _battleService.GetNexus();
                var context = BattleCueContext.Create(monster.ActorId, monster.UnitPosition, nexus.UnitPosition);
                _cuePublisher.Publish(BattleCueMessage.Emit(BattleCueIds.DropSoul, context));
            }

            if (message.ActorId == ActorIds.Tower && message.Tag == StateTags.Die)
            {
                _tower.Value -= 1;
            }
        }
    }
}