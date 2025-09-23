using Domivium.Client.Core.Actors.Contract;
using UnityEngine;

namespace Domivium.Client.Core.Factory
{
    public interface IUnitFactory
    {
        public ActorParam CreateCharacter(int id, Vector3 spawnPosition);
        public ActorParam CreateMonster(int id, Vector3 spawnPosition);
        public ActorParam CreateNexus(int id, Vector3Int spawnPoint);
        public ActorParam CreateTower(int id, Vector3Int spawnPoint);
    }
}