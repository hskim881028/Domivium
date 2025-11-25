using System.Collections.Generic;
using Domivium.Client.Core.Actors;
using Domivium.Client.Core.Actors.Contract;
using Domivium.Client.Core.Battle;
using Domivium.Client.Data.Stat;
using UnityEngine;

namespace Domivium.Client.Core.Factory
{
    public interface IActorParamFactory
    {
        public ActorParam CreateCharacter(
            int id,
            Vector2 spawnPosition,
            IReadOnlyList<BattleAbility> abilities);

        public ActorParam CreateMonster(
            int id,
            Vector2 spawnPosition,
            IReadOnlyList<BattleAbility> abilities,
            IBattleSystem target);

        public ActorParam CreateProjectile(
            int id,
            ActorId target,
            StatSet sourceStatSet,
            Vector2 spawnPosition,
            Vector2 direction,
            IReadOnlyList<BattleAbility> abilities);

        public ActorParam CreateProp(int id, Vector2 spawnPosition);
    }
}