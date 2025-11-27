namespace Domivium.Client.Data.Item
{
    public readonly struct ItemSlotEntry
    {
        public ItemSlotType Type { get; }
        public int Index { get; }

        public ItemSlotEntry(ItemSlotType type, int index)
        {
            Type = type;
            Index = index;
        }

        public bool IsSame(ItemSlotEntry other) => Type == other.Type && Index == other.Index;

        public static ItemSlotEntry Default => new(ItemSlotType.None, -1);
    }
}