using System;
using System.Collections.Generic;
using System.IO;
using Cysharp.Threading.Tasks;
using Domivium.Client.Core.Systems;
using Domivium.Client.Core.Utility;
using Domivium.Client.Data.DataTransferObject;
using Newtonsoft.Json;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Domivium.Client.Contents.Services
{
    public class LocalDataService
    {
        public static string LocalDataDirectory => Path.Combine(Application.persistentDataPath, "local_data");
        private static string UserDataPath(int userId, int characterId) => Path.Combine(LocalDataDirectory, $"user_{userId}_{characterId}.json");
        private static string ItemDataPath(int userId, int characterId) => Path.Combine(LocalDataDirectory, $"items_{userId}_{characterId}.json");
        private static string LootDataPath(int userId, int characterId, int stageId) => Path.Combine(LocalDataDirectory, $"loot_{userId}_{characterId}_{stageId}.json");

        private readonly IStageFieldSystem _stageFieldSystem;
        private readonly MasterDbService _masterDbService;

        public LocalDataService(MasterDbService masterDbService, IStageFieldSystem stageFieldSystem)
        {
            _masterDbService = masterDbService;
            _stageFieldSystem = stageFieldSystem;
            if (!Directory.Exists(LocalDataDirectory))
            {
                Directory.CreateDirectory(LocalDataDirectory);
            }
        }

        public async UniTask<UserDto> LoadUserAsync(int userId, int characterId)
        {
            var path = UserDataPath(userId, characterId);
            if (!File.Exists(path))
            {
                var data = new UserDto { Id = userId, CharacterId = characterId, Level = 1 };
                await SaveUserAsync(data);
                return data;
            }

            var json = await File.ReadAllTextAsync(path);
            return JsonConvert.DeserializeObject<UserDto>(json);
        }

        public async UniTask<ItemsDto> LoadItemAsync(int userId, int characterId)
        {
            var path = ItemDataPath(userId, characterId);
            if (!File.Exists(path))
            {
                var data = new ItemsDto { UserId = userId, CharacterId = characterId };
                await SaveItemAsync(data);
                return data;
            }

            var json = await File.ReadAllTextAsync(path);
            return JsonConvert.DeserializeObject<ItemsDto>(json);
        }

        public async UniTask<LootsDto> LoadLootAsync(int userId, int characterId, int stageId)
        {
            var path = LootDataPath(userId, characterId, stageId);
            if (!File.Exists(path))
            {
                var data = new LootsDto { UserId = userId, CharacterId = characterId, StageId = stageId };
                foreach (var (lootType, loots) in _stageFieldSystem.Loots)
                {
                    foreach (var (lootId, position) in loots)
                    {
                        var items = new List<ItemDto>();
                        var context = _masterDbService.GetLootContext(lootType, lootId);
                        var slotIndex = 0;
                        foreach (var (itemType, itemId, minCount, maxCount, dropRate) in context.Loots)
                        {
                            if (Random.Range(0, 10000) >= dropRate) continue;

                            var item = new ItemDto
                            {
                                SlotIndex = slotIndex,
                                Guid = Guid.NewGuid(),
                                ItemType = itemType,
                                ItemId = itemId,
                                ItemCount = Random.Range(minCount, maxCount + 1),
                                IsStackable = Converter.IsStackable(itemType)
                            };
                            items.Add(item);
                            slotIndex++;
                        }

                        var loot = new LootDto
                        {
                            Guid = Guid.NewGuid(),
                            X = position.x,
                            Y = position.y,
                            LootType = lootType,
                            Capacity = slotIndex,
                            Items = items
                        };

                        data.Entities.Add(loot);
                    }
                }

                await SaveLootAsync(data);
                return data;
            }

            var json = await File.ReadAllTextAsync(path);
            return JsonConvert.DeserializeObject<LootsDto>(json);
        }

        public async UniTask SaveUserAsync(UserDto data)
        {
            var json = JsonConvert.SerializeObject(data, Formatting.Indented);
            await File.WriteAllTextAsync(UserDataPath(data.Id, data.CharacterId), json);
        }

        public async UniTask SaveItemAsync(ItemsDto data)
        {
            var json = JsonConvert.SerializeObject(data, Formatting.Indented);
            await File.WriteAllTextAsync(ItemDataPath(data.UserId, data.CharacterId), json);
        }

        public async UniTask SaveLootAsync(LootsDto data)
        {
            var path = LootDataPath(data.UserId, data.CharacterId, data.StageId);
            var json = JsonConvert.SerializeObject(data, Formatting.Indented);
            await File.WriteAllTextAsync(path, json);
        }
    }
}