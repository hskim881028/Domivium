using System;
using System.Collections.Generic;
using Domivium.Client.Data.Loot;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Domivium.Client.Data.DataTransferObject
{
    [Serializable]
    public class LootDto
    {
        public Guid Guid;
        public float X;
        public float Y;

        [JsonConverter(typeof(StringEnumConverter))]
        public LootType LootType;

        public int Capacity;
        public List<ItemDto> Items = new();
    }

    [Serializable]
    public class LootsDto
    {
        public int UserId;
        public int CharacterId;
        public int StageId;
        public List<LootDto> Entities = new();
        public DateTime Modified;
    }
}