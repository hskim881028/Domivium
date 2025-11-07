using System;
using System.Collections.Generic;
using Domivium.Client.Contents.Actors.Generated;
using Domivium.Client.Contents.Battle;
using Domivium.Client.Contents.Battle.Ability;
using Domivium.Client.Core.Actors;
using Domivium.Client.Core.Battle;
using Domivium.Client.Core.Factory;
using Domivium.Client.Core.Systems;

namespace Domivium.Client.Contents.Factory
{
    public sealed class BattleAbilityFactory : IBattleAbilityFactory
    {
        private readonly IBattleEffectPool _effectPool;
        private readonly IActorParamFactory _actorParamFactory;
        private readonly IActorManager _actorManager;
        private readonly IActorSpawner _actorSpawner;
        private readonly IStageSystem _stageSystem;

        private readonly Dictionary<ActorId, IReadOnlyList<BattleAbility>> _abilities = new();


        public BattleAbilityFactory(
            IBattleEffectPool effectPool,
            IActorParamFactory actorParamFactory,
            IActorManager actorManager,
            IActorSpawner actorSpawner,
            IStageSystem stageSystem)
        {
            _effectPool = effectPool;
            _actorParamFactory = actorParamFactory;
            _actorManager = actorManager;
            _actorSpawner = actorSpawner;
            _stageSystem = stageSystem;

            _abilities.Add(ActorIds.Character,
                new List<BattleAbility>
                {
                    Create(BattleAbilityIds.Turn),
                    Create(BattleAbilityIds.Move),
                    Create(BattleAbilityIds.LookAt),
                    Create(BattleAbilityIds.Attack),
                    Create(BattleAbilityIds.Avoid),
                    Create(BattleAbilityIds.Reload),
                    Create(BattleAbilityIds.CancelReload)
                });

            _abilities.Add(ActorIds.Monster,
                new List<BattleAbility>
                {
                    Create(BattleAbilityIds.Turn),
                    Create(BattleAbilityIds.Move),
                    Create(BattleAbilityIds.LookAt),
                    Create(BattleAbilityIds.Attack),
                    Create(BattleAbilityIds.Avoid),
                    Create(BattleAbilityIds.Reload)
                });

            _abilities.Add(ActorIds.Projectile,
                new List<BattleAbility>
                {
                    Create(BattleAbilityIds.Turn),
                    Create(BattleAbilityIds.Tracking),
                    Create(BattleAbilityIds.Attack),
                });
        }

        public IReadOnlyList<BattleAbility> GetAbilities(ActorId actorId) => _abilities[actorId];

        private BattleAbility Create(BattleAbilityId id)
        {
            if (id == BattleAbilityIds.Turn)
            {
                return new TurnAbility(_effectPool);
            }

            if (id == BattleAbilityIds.Move)
            {
                return new MoveAbility(_effectPool, _stageSystem);
            }

            if (id == BattleAbilityIds.Avoid)
            {
                return new AvoidAbility(_effectPool, _stageSystem);
            }

            if (id == BattleAbilityIds.LookAt)
            {
                return new LookAtAbility(_effectPool);
            }

            if (id == BattleAbilityIds.Attack)
            {
                return new AttackAbility(_effectPool, this, _actorParamFactory, _actorSpawner);
            }

            if (id == BattleAbilityIds.Heal)
            {
                return new HealAbility(_effectPool);
            }

            if (id == BattleAbilityIds.Tracking)
            {
                return new TrackingAbility(_effectPool, _actorManager);
            }

            if (id == BattleAbilityIds.Reload)
            {
                return new ReloadAbility(_effectPool);
            }

            if (id == BattleAbilityIds.CancelReload)
            {
                return new CancelReloadAbility(_effectPool);
            }

            throw new Exception($"Invalid battle ability: {id}");
        }
    }
}