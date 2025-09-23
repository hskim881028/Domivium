using System;
using Domivium.Client.Contents.Battle;
using Domivium.Client.Contents.Battle.Ability;
using Domivium.Client.Core.Battle;
using Domivium.Client.Core.Factory;
using Domivium.Client.Core.Message;
using MessagePipe;

namespace Domivium.Client.Contents.Factory
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

        public BattleAbility Create(BattleAbilityId id)
        {
            if (id == BattleAbilityIds.Slash)
            {
                return new SlashAbility(_effectPool);
            }

            throw new Exception($"Invalid battle ability: {id}");
        }
    }
}