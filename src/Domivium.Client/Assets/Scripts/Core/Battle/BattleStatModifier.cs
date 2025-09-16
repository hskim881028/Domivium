using Domivium.Client.Data.Stat;

namespace Domivium.Client.Core.Battle
{
    public readonly struct BattleStatModifier
    {
        public StatId Id { get; }
        public int Value { get; }
        public StatChannel Channel { get; }

        public BattleStatModifier(StatId id, int value, StatChannel channel)
        {
            Id = id;
            Value = value;
            Channel = channel;
        }
    }
}