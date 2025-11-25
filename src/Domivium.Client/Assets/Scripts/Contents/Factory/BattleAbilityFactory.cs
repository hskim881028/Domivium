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
        private readonly IStageFieldSystem _stageFieldSystem;

        private readonly Dictionary<ActorId, IReadOnlyList<BattleAbility>> _abilities = new();


        public BattleAbilityFactory(
            IBattleEffectPool effectPool,
            IActorParamFactory actorParamFactory,
            IActorManager actorManager,
            IActorSpawner actorSpawner,
            IStageFieldSystem stageFieldSystem)
        {
            _effectPool = effectPool;
            _actorParamFactory = actorParamFactory;
            _actorManager = actorManager;
            _actorSpawner = actorSpawner;
            _stageFieldSystem = stageFieldSystem;

            _abilities.Add(ActorId.Character,
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

            _abilities.Add(ActorId.Monster,
                new List<BattleAbility>
                {
                    Create(BattleAbilityIds.Turn),
                    Create(BattleAbilityIds.Move),
                    Create(BattleAbilityIds.LookAt),
                    Create(BattleAbilityIds.Attack),
                    Create(BattleAbilityIds.Avoid),
                    Create(BattleAbilityIds.Reload),
                    Create(BattleAbilityIds.Chase)
                });

            _abilities.Add(ActorId.Projectile,
                new List<BattleAbility>
                {
                    Create(BattleAbilityIds.Turn),
                    Create(BattleAbilityIds.Tracking),
                    Create(BattleAbilityIds.Attack)
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
                return new MoveAbility(_effectPool, _stageFieldSystem);
            }

            if (id == BattleAbilityIds.Avoid)
            {
                return new AvoidAbility(_effectPool, _stageFieldSystem);
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
            if (id == BattleAbilityIds.Chase)
            {
                return new ChaseAbility(_effectPool, _stageFieldSystem);
            }

            throw new Exception($"Invalid battle ability: {id}");
        }
    }
}