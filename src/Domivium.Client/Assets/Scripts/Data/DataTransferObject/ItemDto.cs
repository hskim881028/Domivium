using System;
using System.Collections.Generic;
using Domivium.Client.Data.Item;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Domivium.Client.Data.DataTransferObject
{
    [Serializable]
    public class ItemSlotSaveData
    {
        public int SlotIndex;
        public Guid Guid;

        [JsonConverter(typeof(StringEnumConverter))]
        public ItemType ItemType;

        public int ItemId;
        public int ItemCount;
    }

    [Serializable]
    public class InventorySaveData
    {
        public int InventoryCapacity;
        public int StorageCapacity;

        public List<ItemSlotSaveData> Equipment = new();
        public List<ItemSlotSaveData> Inventory = new();
        public List<ItemSlotSaveData> Storage = new();
    }
}