using System;
using System.Collections.Generic;
using Domivium.Client.Contents.Actors;
using Domivium.Client.Contents.Actors.Contract;
using Domivium.Client.Contents.Actors.Generated;
using Domivium.Client.Contents.Battle;
using Domivium.Client.Contents.Services;
using Domivium.Client.Core.Actors;
using Domivium.Client.Core.Actors.Contract;
using Domivium.Client.Core.Battle;
using Domivium.Client.Core.Factory;
using UnityEngine;

namespace Domivium.Client.Contents.Factory
{
    public sealed class ActorFactory : IActorFactory
    {
        private readonly MasterDbService _masterDbService;
        private readonly IBattleAbilityFactory _abilityFactory;

        public ActorFactory(MasterDbService masterDbService, IBattleAbilityFactory abilityFactory)
        {
            _masterDbService = masterDbService;
            _abilityFactory = abilityFactory;
        }

        public ActorParam CreateCharacter(int id, Vector3Int spawnPoint)
        {
            var row = _masterDbService.DB.CharacterRowTable.FindById(id);
            var abilities = GetDefaultAbility(row.Job.ToPawnType());
            var context = new PawnContext(row);
            return new UnitParams(spawnPoint, context, abilities);
        }

        public ActorParam CreateMonster(int id, Vector3Int spawnPoint)
        {
            var row = _masterDbService.DB.MonsterRowTable.FindById(id);
            var abilities = GetDefaultAbility(row.Job.ToPawnType());
            var context = new PawnContext(row);
            return new UnitParams(spawnPoint, context, abilities);
        }

        public ActorParam CreateTower(int id, Vector3Int spawnPoint)
        {
            var row = _masterDbService.DB.TowerRowTable.FindById(id);
            var abilities = GetDefaultAbility(row.Job.ToPawnType());
            var context = new PawnContext(row);
            return new UnitParams(spawnPoint, context, abilities);
        }

        public ActorParam CreateNexus(int id, Vector3Int spawnPoint)
        {
            var row = _masterDbService.DB.NexusRowTable.FindById(id);
            var context = new PawnContext(row);
            return new UnitParams(spawnPoint, context, new List<BattleAbility>());
        }

        public ActorParam CreateMonsterCamp(int index, Vector3Int spawnPoint)
        {
            return new CampParams(index, spawnPoint);
        }

        private List<BattleAbility> GetDefaultAbility(PawnType pawnType) // temp
        {
            var abilities = new List<BattleAbility>();
            if (pawnType == PawnTypes.Melee)
            {
                abilities.Add(_abilityFactory.Create(BattleAbilityIds.Attack));
            }

            if (pawnType == PawnTypes.Ranged)
            {
                abilities.Add(_abilityFactory.Create(BattleAbilityIds.Attack));
            }

            if (pawnType == PawnTypes.Tank)
            {
                abilities.Add(_abilityFactory.Create(BattleAbilityIds.Attack));
            }

            if (pawnType == PawnTypes.Support)
            {
                abilities.Add(_abilityFactory.Create(BattleAbilityIds.Heal));
            }

            return abilities;
        }
    }
}