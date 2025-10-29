using Domivium.Client.Core.Actors.Contract;
using UnityEngine;

namespace Domivium.Client.Core.Factory
{
    public interface IActorFactory
    {
        public ActorParam CreateCharacter(int id, Vector3Int spawnPoint);
        public ActorParam CreateMonster(int id, Vector3Int spawnPoint);
        public ActorParam CreateMonsterCamp(int index, Vector3Int spawnPoint);
    }
}