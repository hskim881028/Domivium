using System;
using System.Collections.Generic;
using System.Linq;
using Domivium.Client.Contents.Actors;
using Domivium.Client.Contents.Actors.Contract;
using Domivium.Client.Contents.Battle;
using Domivium.Client.Contents.Services;
using Domivium.Client.Core.Actors.Contract;
using Domivium.Client.Core.Actors.Unit;
using Domivium.Client.Core.Battle;
using Domivium.Client.Core.Factory;
using UnityEngine;

namespace Domivium.Client.Contents.Factory
{
    public sealed class UnitFactory : IUnitFactory
    {
        private readonly MasterDbService _masterDbService;
        private readonly CoordinateService _coordinateService;
        private readonly IBattleAbilityFactory _abilityFactory;

        public UnitFactory(
            MasterDbService masterDbService,
            CoordinateService coordinateService,
            IBattleAbilityFactory abilityFactory)
        {
            _masterDbService = masterDbService;
            _coordinateService = coordinateService;
            _abilityFactory = abilityFactory;
        }

        public ActorParam CreateCharacter(int id, Vector3 spawnPosition)
        {
            var row = _masterDbService.DB.CharacterRowTable.FindById(id);
            var abilities = GetDefaultAbility(row.Job.ToUnitType());
            var context = new UnitContext(row);
            var spawnPoint = _coordinateService.GetPosition(spawnPosition);
            return new UnitParams(spawnPoint, context, abilities);
        }

        public ActorParam CreateMonster(int id, Vector3 spawnPosition)
        {
            var row = _masterDbService.DB.MonsterRowTable.FindById(id);
            var abilities = GetDefaultAbility(row.Job.ToUnitType());
            var context = new UnitContext(row);
            var spawnPoint = _coordinateService.GetPosition(spawnPosition);
            return new UnitParams(spawnPoint, context, abilities);
        }

        public ActorParam CreateNexus(int id, Vector3Int spawnPoint)
        {
            var row = _masterDbService.DB.NexusRowTable.FindById(id);
            var context = new UnitContext(row);
            return new UnitParams(spawnPoint, context, new List<BattleAbility>());
        }

        public ActorParam CreateTower(int id, Vector3Int spawnPoint)
        {
            var row = _masterDbService.DB.TowerRowTable.FindById(id);
            var abilities = GetDefaultAbility(row.Job.ToUnitType());
            var context = new UnitContext(row);
            return new UnitParams(spawnPoint, context, abilities);
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