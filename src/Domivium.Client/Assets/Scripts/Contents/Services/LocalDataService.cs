using System.IO;
using Domivium.Client.Contents.DI;
using Domivium.Client.Core;
using Domivium.Client.Data.DataTransferObject;
using Domivium.Client.Data.Item;
using Newtonsoft.Json;
using UnityEngine;

namespace Domivium.Client.Contents.Services
{
    public class LocalDataService
    {
        private readonly IAppContext _appContext;
        public static string SavePath => Path.Combine(Application.persistentDataPath, FileName);

        private const string FileName = "items.json";

        public LocalDataService(IAppContext appContext)
        {
            _appContext = appContext;
        }

        public void Save(Items items)
        {
            if (_appContext.Scene.CurrentValue != SceneScopeIds.Lobby) return;

            var data = new InventorySaveData
            {
                InventoryCapacity = items.InventoryCapacity.CurrentValue,
                StorageCapacity = items.StorageCapacity.CurrentValue
            };

            foreach (var kvp in items.Equipment)
            {
                data.Equipment.Add(new ItemSlotSaveData
                {
                    SlotIndex = kvp.Key,
                    Guid = kvp.Value.Guid,
                    ItemType = kvp.Value.Type,
                    ItemId = kvp.Value.Id,
                    ItemCount = kvp.Value.Count
                });
            }

            foreach (var kvp in items.Inventory)
            {
                data.Inventory.Add(new ItemSlotSaveData
                {
                    SlotIndex = kvp.Key,
                    Guid = kvp.Value.Guid,
                    ItemType = kvp.Value.Type,
                    ItemId = kvp.Value.Id,
                    ItemCount = kvp.Value.Count
                });
            }

            foreach (var kvp in items.Storage)
            {
                data.Storage.Add(new ItemSlotSaveData
                {
                    SlotIndex = kvp.Key,
                    Guid = kvp.Value.Guid,
                    ItemType = kvp.Value.Type,
                    ItemId = kvp.Value.Id,
                    ItemCount = kvp.Value.Count
                });
            }

            var json = JsonConvert.SerializeObject(data, Formatting.Indented);
            File.WriteAllText(SavePath, json);
        }

        public bool Load(ref Items items)
        {
            if (!File.Exists(SavePath)) return false;

            var json = File.ReadAllText(SavePath);
            var data = JsonConvert.DeserializeObject<InventorySaveData>(json);
            if (data == null) return false;

            items.InventoryCapacity.Value = data.InventoryCapacity;
            items.StorageCapacity.Value = data.StorageCapacity;

            items.Equipment.Clear();
            foreach (var item in data.Equipment)
            {
                items.Equipment[item.SlotIndex] = new ItemEntity(item.Guid, item.ItemType, item.ItemId, item.ItemCount);
            }

            items.Inventory.Clear();
            foreach (var item in data.Inventory)
            {
                items.Inventory[item.SlotIndex] = new ItemEntity(item.Guid, item.ItemType, item.ItemId, item.ItemCount);
            }

            items.Storage.Clear();
            foreach (var item in data.Storage)
            {
                items.Storage[item.SlotIndex] = new ItemEntity(item.Guid, item.ItemType, item.ItemId, item.ItemCount);
            }

            return true;
        }
    }
}