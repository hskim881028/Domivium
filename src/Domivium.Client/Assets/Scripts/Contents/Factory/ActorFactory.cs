using System;
using System.Collections.Generic;
using Domivium.Client.Contents.Actors;
using Domivium.Client.Contents.Actors.Contract;
using Domivium.Client.Contents.Actors.Generated;
using Domivium.Client.Contents.Battle;
using Domivium.Client.Contents.Services;
using Domivium.Client.Core.Actors;
using Domivium.Client.Core.Actors.Contract;
using Domivium.Client.Core.Actors.Unit;
using Domivium.Client.Core.Battle;
using Domivium.Client.Core.Factory;
using Domivium.Client.Core.Provider;
using UnityEngine;

namespace Domivium.Client.Contents.Factory
{
    public sealed class ActorFactory : IActorFactory
    {
        private readonly MasterDbService _masterDbService;
        private readonly CoordinateService _coordinateService;
        private readonly StageMapProvider _stageMapProvider;
        private readonly IBattleAbilityFactory _abilityFactory;

        public ActorFactory(
            MasterDbService masterDbService,
            CoordinateService coordinateService,
            StageMapProvider stageMapProvider,
            IBattleAbilityFactory abilityFactory)
        {
            _masterDbService = masterDbService;
            _coordinateService = coordinateService;
            _stageMapProvider = stageMapProvider;
            _abilityFactory = abilityFactory;
        }

        public ActorParam CreateCharacter(int id, Vector3 spawnPosition)
        {
            var row = _masterDbService.DB.CharacterRowTable.FindById(id);
            var abilities = GetDefaultAbility(row.Job.ToUnitType());
            var context = new UnitContext(row);
            var spawnPoint = _coordinateService.GetCellPoint(spawnPosition);
            return new UnitParams(spawnPoint, context, abilities);
        }

        public ActorParam CreateMonster(int id, Vector3Int spawnPoint)
        {
            var row = _masterDbService.DB.MonsterRowTable.FindById(id);
            var abilities = GetDefaultAbility(row.Job.ToUnitType());
            var context = new UnitContext(row);
            return new UnitParams(spawnPoint, context, abilities);
        }

        public ActorParam CreateTower(int id, Vector3Int spawnPoint)
        {
            var row = _masterDbService.DB.TowerRowTable.FindById(id);
            var abilities = GetDefaultAbility(row.Job.ToUnitType());
            var context = new UnitContext(row);
            return new UnitParams(spawnPoint, context, abilities);
        }

        public ActorParam CreateCamp(
            int stageId,
            ActorId actorId,
            int index,
            Vector3Int spawnPoint)
        {
            if (actorId == ActorIds.Nexus)
            {
                var row = _masterDbService.DB.NexusRowTable.FindById(1);
                var context = new UnitContext(row);
                return new UnitParams(spawnPoint, context, new List<BattleAbility>());
            }

            if (actorId == ActorIds.CharacterCamp || actorId == ActorIds.MonsterCamp)
            {
                return CreateCamp(stageId, index, spawnPoint);
            }

            throw new Exception("Invalid actor");
        }

        private ActorParam CreateCamp(int stageId, int index, Vector3Int spawnPoint)
        {
            var ground = _stageMapProvider.Get(stageId).cellBounds;
            var xMin = ground.xMin;
            var xMax = ground.xMax - 1;
            var yMin = ground.yMin;
            var yMax = ground.yMax - 1;

            if (spawnPoint.x < xMin) // left
            {
                return new CampParams(index, spawnPoint, new Vector3(1.5f, -0.1f, 0.5f), new Vector3(0.6f, 1, 0.4f));
            }

            if (spawnPoint.x > xMax) // right
            {
                return new CampParams(index, spawnPoint, new Vector3(-1.5f, -0.1f, 0.5f), new Vector3(0.6f, 1, 0.4f));
            }

            if (spawnPoint.y < yMin) // bottom
            {
                return new CampParams(index, spawnPoint, new Vector3(0, -0.1f, 2), new Vector3(0.4f, 1, 0.6f));
            }

            if (spawnPoint.y > yMax) // top
            {
                return new CampParams(index, spawnPoint, new Vector3(0, -0.1f, -1), new Vector3(0.4f, 1, 0.6f));
            }

            throw new Exception("Invalid camp spawn position");
        }

        private List<BattleAbility> GetDefaultAbility(UnitType unitType) // temp
        {
            var abilities = new List<BattleAbility>();
            if (unitType == UnitTypes.Melee)
            {
                abilities.Add(_abilityFactory.Create(BattleAbilityIds.Attack));
            }

            if (unitType == UnitTypes.Ranged)
            {
                abilities.Add(_abilityFactory.Create(BattleAbilityIds.Attack));
            }

            if (unitType == UnitTypes.Tank)
            {
                abilities.Add(_abilityFactory.Create(BattleAbilityIds.Attack));
            }

            if (unitType == UnitTypes.Support)
            {
                abilities.Add(_abilityFactory.Create(BattleAbilityIds.Heal));
            }

            return abilities;
        }
    }
}