namespace Domivium.Client.Core.Battle
{
    public struct BattleEffectContext
    {
        public BattleEffectId EffectId { get; private init; }
        public IBattleSystem Source { get; private init; } // 공격자
        public IBattleSystem Owner { get; private init; } // 방어자

        // 공격자 무기
        // 공격자 총알
        // 방어자 방어구

        public int Value { get; set; }

        public static BattleEffectContext Create(BattleEffectId effectId, IBattleSystem source, IBattleSystem owner) => new()
        {
            EffectId = effectId,
            Source = source,
            Owner = owner
        };
    }
}