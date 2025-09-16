using Domivium.Client.Data.Stat;

namespace Domivium.Client.Core.Battle
{
    public readonly struct BattleGaugeModifier
    {
        public StatId Id { get; }
        public int Value { get; }
        public GaugeChannel Channel { get; }

        public BattleGaugeModifier(StatId id, int value, GaugeChannel channel)
        {
            Id = id;
            Value = value;
            Channel = channel;
        }
    }
}