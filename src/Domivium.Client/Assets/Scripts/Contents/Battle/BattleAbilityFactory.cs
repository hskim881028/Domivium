using System;
using Domivium.Client.Contents.Battle.Ability;
using Domivium.Client.Core.Battle;
using Domivium.Client.Core.Message;
using MessagePipe;

namespace Domivium.Client.Contents.Battle
{
    public sealed class BattleAbilityFactory : IBattleAbilityFactory
    {
        private readonly IBattleEffectPool _effectPool;
        private readonly IPublisher<BattleCueMessage> _cuePublisher;

        public BattleAbilityFactory(IBattleEffectPool effectPool, IPublisher<BattleCueMessage> cuePublisher)
        {
            _effectPool = effectPool;
            _cuePublisher = cuePublisher;
        }

        public BattleAbilitySpec Create(BattleAbilityId id)
        {
            var ability = CreateAbility(id);
            return new BattleAbilitySpec(ability, _cuePublisher);
        }

        private BattleAbility CreateAbility(BattleAbilityId id)
        {
            if (id == BattleAbilityIds.Slash)
            {
                return new SlashAbility(_effectPool);
            }

            throw new Exception($"Invalid battle ability: {id}");
        }
    }
}