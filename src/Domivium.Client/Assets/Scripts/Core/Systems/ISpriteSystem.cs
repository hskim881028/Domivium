using Domivium.Client.Core.Actors;
using Domivium.Client.Data.Item;
using UnityEngine;

namespace Domivium.Client.Core.Systems
{
    public interface ISpriteSystem
    {
        public Sprite GetItem(ItemType itemType, int id);
        public Sprite GetProjectile(ActorId actorId, int id);
    }
}