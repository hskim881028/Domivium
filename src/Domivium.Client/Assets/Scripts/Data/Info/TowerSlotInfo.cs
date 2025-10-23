namespace Domivium.Client.Data.Info
{
    public readonly struct TowerSlotInfo
    {
        public int TowerId { get; }
        public int Cost { get; }

        private TowerSlotInfo(int towerId, int cost)
        {
            TowerId = towerId;
            Cost = cost;
        }

        public static TowerSlotInfo Create(int towerId, int cost) => new(towerId, cost);
    }
}