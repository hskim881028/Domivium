using System;
using System.Collections.Generic;
using Domivium.Client.Data.DataTransferObject;
using Domivium.Client.Data.Item;
using UnityEngine;

namespace Domivium.Client.Data.Loot
{
    public sealed class Loots
    {
        private const float DetectRange = 1;

        private readonly List<LootEntity> _loots = new();

        private Transform _character;
        private int _userId;
        private int _characterId;
        private int _stageId;
        private bool _confirmed;

        public void SetData(Transform character, LootsDto dto)
        {
            _confirmed = true;
            _character = character;

            _userId = dto.UserId;
            _characterId = dto.CharacterId;
            _stageId = dto.StageId;

            _loots.Clear();
            foreach (var entityDto in dto.Entities)
            {
                var items = new Dictionary<int, ItemEntity>();
                foreach (var itemDto in entityDto.Items)
                {
                    var item = new ItemEntity(itemDto.Guid, itemDto.ItemType, itemDto.ItemId, itemDto.ItemCount, itemDto.IsStackable);
                    items[itemDto.SlotIndex] = item;
                }

                var entity = new LootEntity(
                    entityDto.Guid,
                    new Vector2(entityDto.X, entityDto.Y),
                    entityDto.LootType,
                    items,
                    entityDto.Capacity
                );

                _loots.Add(entity);
            }
        }

        public LootEntity FindNearestLoot()
        {
            LootEntity nearestLoot = null;
            var nearestDist = DetectRange;
            foreach (var loot in _loots)
            {
                var dist = Vector2.Distance(_character.position, loot.Position);
                if (dist < nearestDist)
                {
                    nearestDist = dist;
                    nearestLoot = loot;
                }
            }

            return nearestLoot;
        }

        public bool ToDto(out LootsDto data)
        {
            data = new LootsDto();

            if (!_confirmed) return false;

            data.Modified = DateTime.UtcNow;
            data.UserId = _userId;
            data.CharacterId = _characterId;
            data.StageId = _stageId;

            foreach (var loot in _loots)
            {
                var entityDto = new LootDto
                {
                    Guid = loot.Guid,
                    X = loot.Position.x,
                    Y = loot.Position.y,
                    LootType = loot.Type,
                    Capacity = loot.Capacity
                };

                foreach (var (slotIndex, item) in loot.Items)
                {
                    entityDto.Items.Add(new ItemDto
                    {
                        SlotIndex = slotIndex,
                        Guid = item.Guid,
                        ItemType = item.Type,
                        ItemId = item.Id,
                        ItemCount = item.Count,
                        IsStackable = item.IsStackable
                    });
                }

                data.Entities.Add(entityDto);
            }

            return true;
        }
    }
}