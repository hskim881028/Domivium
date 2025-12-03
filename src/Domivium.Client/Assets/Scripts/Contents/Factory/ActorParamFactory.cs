using System.Collections.Generic;
using Domivium.Client.Contents.Actors.Contract;
using Domivium.Client.Contents.Services;
using Domivium.Client.Core.Actors;
using Domivium.Client.Core.Actors.Contract;
using Domivium.Client.Core.Battle;
using Domivium.Client.Core.Factory;
using Domivium.Client.Data.Context;
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

        public ActorParam CreateLobbyCharacter(int id, Vector2 spawnPosition, IReadOnlyList<BattleAbility> abilities)
        {
            var characterRow = _masterDbService.DB.CharacterRowTable.FindById(id);
            var unit = new UnitContext(characterRow);
            return new LobbyCharacterParams(spawnPosition, unit, abilities);
        }

        public ActorParam CreateCharacter(
            int id,
            Vector2 spawnPosition,
            IReadOnlyList<BattleAbility> abilities)
        {
            var characterRow = _masterDbService.DB.CharacterRowTable.FindById(id);
            var unit = new UnitContext(characterRow);
            return new CharacterParams(spawnPosition, unit, abilities);
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
            StatSet sourceStatSet,
            ActorId sourceActorId,
            ActorId targetActorId,
            Vector2 spawnPosition,
            Vector2 direction,
            IReadOnlyList<BattleAbility> abilities) => new ProjectileParams(id, sourceStatSet, sourceActorId, targetActorId, spawnPosition, direction, abilities);

        public ActorParam CreateProp(int id, Vector2 spawnPosition) => new PropParams(spawnPosition);
    }
}