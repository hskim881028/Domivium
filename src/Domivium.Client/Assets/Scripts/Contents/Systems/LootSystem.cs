using System.Collections.Generic;
using Domivium.Client.Core.Actors;
using Domivium.Client.Core.Systems;
using R3;
using UnityEngine;

namespace Domivium.Client.Contents.Systems
{
    public sealed class LootSystem : Disposable, ILootSystem, ILootSystemCommand
    {
        private const float DetectRange = 1;
        private Transform _character;
        private IReadOnlyDictionary<ushort, Vector2> _stageProps;
        private ushort _lootId;

        private readonly ReactiveCommand<ushort> _onFind = new();

        public ReactiveCommand<ushort> OnFind => _onFind;

        public LootSystem(IActorManager actorManager)
        {
            actorManager.Character.Subscribe(OnChangeCharacter).AddTo(ref DisposableBag);
        }

        public void Initialize(IReadOnlyDictionary<ushort, Vector2> stageProps)
        {
            _stageProps = stageProps;
        }

        public void Tick(float deltaTime)
        {
            var propId = FindNearestProp();
            if (_lootId == propId) return;

            _lootId = propId;
            _onFind.Execute(propId);
        }

        private ushort FindNearestProp()
        {
            ushort lootId = 0;
            var nearestDist = DetectRange;
            foreach (var (id, position) in _stageProps)
            {
                var dist = Vector3.Distance(_character.position, position);
                if (dist < nearestDist)
                {
                    nearestDist = dist;
                    lootId = id;
                }
            }

            return lootId;
        }

        private void OnChangeCharacter(IUnitPresenter character)
        {
            if (character == null) return;

            _character = character.Transform;
        }
    }
}