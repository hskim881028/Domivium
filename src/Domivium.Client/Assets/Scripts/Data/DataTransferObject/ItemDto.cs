using System;
using System.Collections.Generic;
using Domivium.Client.Data.Item;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Domivium.Client.Data.DataTransferObject
{
    [Serializable]
    public class ItemDto
    {
        public int SlotIndex;
        public Guid Guid;

        [JsonConverter(typeof(StringEnumConverter))]
        public ItemType ItemType;

        public int ItemId;
        public int ItemCount;
        public bool IsStackable;
    }

    [Serializable]
    public class ItemsDto
    {
        public int UserId;
        public int CharacterId;

        public List<ItemDto> Equipment = new();
        public List<ItemDto> Inventory = new();
        public List<ItemDto> Storage = new();
        public DateTime Modified;
    }
}