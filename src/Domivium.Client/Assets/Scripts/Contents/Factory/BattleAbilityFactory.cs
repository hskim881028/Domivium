using System;
using Domivium.Client.Contents.Battle;
using Domivium.Client.Contents.Battle.Ability;
using Domivium.Client.Core.Actors;
using Domivium.Client.Core.Battle;
using Domivium.Client.Core.Factory;
using Domivium.Client.Core.Message;
using MessagePipe;

namespace Domivium.Client.Contents.Factory
{
    public sealed class BattleAbilityFactory : IBattleAbilityFactory
    {
        private readonly IBattleEffectPool _effectPool;
        private readonly IActorFinder _actorFinder;
        private readonly IPublisher<BattleCueMessage> _cuePublisher;

        public BattleAbilityFactory(
            IBattleEffectPool effectPool,
            IActorFinder actorFinder,
            IPublisher<BattleCueMessage> cuePublisher)
        {
            _effectPool = effectPool;
            _actorFinder = actorFinder;
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
                return new SlashAbility(_effectPool, _actorFinder);
            }

            throw new Exception($"Invalid battle ability: {id}");
        }
    }
}