namespace Domivium.Client.Data.Item
{
    public readonly struct ItemSlotData
    {
        public ItemSlotType Type { get; }
        public int Index { get; }

        public ItemSlotData(ItemSlotType type, int index)
        {
            Type = type;
            Index = index;
        }

        public bool IsSame(ItemSlotData other)
        {
            return Type == other.Type && Index == other.Index;
        }

        public static ItemSlotData Default => new(ItemSlotType.None, -1);
    }
}