using System.Collections.Generic;
using Domivium.Client.Core.Systems;
using R3;
using UnityEngine;

namespace Domivium.Client.Contents.Systems
{
    public sealed class LootSystem : Disposable, ILootSystem, ILootSystemCommand
    {
        private const float DetectRange = 1;

        private readonly ReactiveCommand<ushort> _onFind = new();
        private Transform _character;
        private IReadOnlyDictionary<ushort, Vector2> _stageProps;
        private ushort _lootId;
        
        // todo: 첫 세팅시키기.

        public ReactiveCommand<ushort> OnFind => _onFind;

        public void Initialize(Transform character, IReadOnlyDictionary<ushort, Vector2> stageProps)
        {
            _character = character;
            _stageProps = stageProps;
        }

        public void Tick()
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
    }
}