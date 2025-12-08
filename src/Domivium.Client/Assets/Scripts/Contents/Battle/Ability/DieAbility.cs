using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Domivium.Client.Core.Actors;
using Domivium.Client.Core.Battle;
using Domivium.Client.Core.Container;
using Domivium.Client.Core.Factory;
using Domivium.Client.Core.State;
using Domivium.Client.Core.Utility;

namespace Domivium.Client.Contents.Battle.Ability
{
    public class DieAbility : BattleAbility
    {
        private readonly IUserContainer _userContainer;
        private readonly IActorParamFactory _actorParamFactory;
        private readonly IActorSpawner _actorSpawner;
        public override BattleAbilityId Id => BattleAbilityIds.Die;
        public override BattleCueId CueId => BattleCueIds.Die;

        protected override IReadOnlyCollection<StateTag> BlockedStateTags => TagGenerator.EmptyStateTag;

        public DieAbility(
            IBattleEffectPool effectPool,
            IUserContainer userContainer,
            IActorParamFactory actorParamFactory,
            IActorSpawner actorSpawner) : base(effectPool)
        {
            _userContainer = userContainer;
            _actorParamFactory = actorParamFactory;
            _actorSpawner = actorSpawner;
        }

        public override float Activate(ref BattleAbilityContext context)
        {
            var position = context.Source.Position;
            if (_userContainer.IsExistLoot(position))
            {
                var add = MathUtils.Random(0.1f, 0.5f);
                position += add;
            }

            if (context.Source.ActorId == ActorId.Character)
            {
                _userContainer.Die(position);
            }
            else if (context.Source.ActorId == ActorId.Monster)
            {
                _userContainer.AddMonsterBox(context.Source.Id, position);
            }

            var param = _actorParamFactory.CreateProp(1, position);
            _actorSpawner.SpawnAsync(ActorId.Prop, param, 1).Forget();
            return 99;
        }
    }
}