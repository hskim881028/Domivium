using System.Collections.Generic;
using Domivium.Client.Contents.Actors;
using Domivium.Client.Contents.Actors.Contract;
using Domivium.Client.Contents.Item;
using Domivium.Client.Contents.Services;
using Domivium.Client.Core.Actors;
using Domivium.Client.Core.Actors.Contract;
using Domivium.Client.Core.Battle;
using Domivium.Client.Core.Factory;
using Domivium.Client.Data.Stat;
using UnityEngine;

namespace Domivium.Client.Contents.Factory
{
    public sealed class ActorParamFactory : IActorParamFactory
    {
        private readonly MasterDbService _masterDbService;

        public ActorParamFactory(MasterDbService masterDbService)
        {
            _masterDbService = masterDbService;
        }

        public ActorParam CreateCharacter(
            int id,
            Vector2 spawnPosition,
            IReadOnlyList<BattleAbility> abilities)
        {
            var characterRow = _masterDbService.DB.CharacterRowTable.FindById(id);
            var unit = new UnitContext(characterRow);

            var weaponRow = _masterDbService.DB.WeaponRowTable.FindById(id);
            var weapon = new ItemContext(weaponRow);
            return new CharacterParams(spawnPosition, unit, weapon, abilities);
        }

        public ActorParam CreateMonster(
            int id,
            Vector2 spawnPosition,
            IReadOnlyList<BattleAbility> abilities,
            IBattleSystem target)
        {
            var row = _masterDbService.DB.MonsterRowTable.FindById(id);
            var context = new UnitContext(row);
            return new MonsterParams(spawnPosition, context, abilities, target);
        }

        public ActorParam CreateProjectile(
            int id,
            ActorId target,
            StatSet sourceStatSet,
            Vector2 spawnPosition,
            Vector2 direction,
            IReadOnlyList<BattleAbility> abilities)
        {
            var projectileRow = _masterDbService.DB.ProjectileRowTable.FindById(id);
            var projectileContext = new ProjectileContext(projectileRow);
            return new ProjectileParams(target, sourceStatSet, spawnPosition, direction, projectileContext, abilities);
        }
    }
}