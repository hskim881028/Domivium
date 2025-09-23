using System.Collections.Generic;
using Domivium.Client.Contents.Actors.Contract;
using Domivium.Client.Contents.Battle;
using Domivium.Client.Contents.Services;
using Domivium.Client.Core.Actors.Contract;
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
            var ability = _abilityFactory.Create(BattleAbilityIds.Slash);
            var abilities = new List<BattleAbility> { ability };
            var context = new UnitContext(row);
            var spawnPoint = _coordinateService.GetPosition(spawnPosition);
            return new UnitParams(spawnPoint, context, abilities);
        }

        public ActorParam CreateMonster(int id, Vector3 spawnPosition)
        {
            var row = _masterDbService.DB.MonsterRowTable.FindById(id);
            var ability = _abilityFactory.Create(BattleAbilityIds.Slash);
            var abilities = new List<BattleAbility> { ability };
            var context = new UnitContext(row);
            var spawnPoint = _coordinateService.GetPosition(spawnPosition);
            return new UnitParams(spawnPoint, context, abilities);
        }

        public ActorParam CreateNexus(int id, Vector3Int spawnPoint)
        {
            var row = _masterDbService.DB.NexusRowTable.FindById(id);
            var ability = _abilityFactory.Create(BattleAbilityIds.Slash);
            var abilities = new List<BattleAbility> { ability };
            var context = new UnitContext(row);
            return new UnitParams(spawnPoint, context, abilities);
        }

        public ActorParam CreateTower(int id, Vector3Int spawnPoint)
        {
            var row = _masterDbService.DB.TowerRowTable.FindById(id);
            var ability = _abilityFactory.Create(BattleAbilityIds.Slash);
            var abilities = new List<BattleAbility> { ability };
            var context = new UnitContext(row);
            return new UnitParams(spawnPoint, context, abilities);
        }
    }
}