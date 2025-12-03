using System.Collections.Generic;
using System.Linq;
using Domivium.Client.Core.Actors;
using Domivium.Client.Core.Systems;
using Domivium.Client.Data.Item;
using UnityEngine;

namespace Domivium.Client.Contents.Systems
{
    public class SpriteSystem : ISpriteSystem
    {
        private readonly Sprite _none;
        private readonly IReadOnlyDictionary<ItemType, IReadOnlyDictionary<int, Sprite>> _items;
        private readonly IReadOnlyDictionary<ActorId, IReadOnlyDictionary<int, Sprite>> _projectiles;

        public SpriteSystem(
            Dictionary<ItemType, List<Sprite>> itemSprites,
            Dictionary<ActorId, List<Sprite>> projectileSprites)
        {
            _none = itemSprites[ItemType.None].First();
            var items = new Dictionary<ItemType, IReadOnlyDictionary<int, Sprite>>();
            foreach (var (itemType, sprites) in itemSprites)
            {
                var id = 1;
                var item = new Dictionary<int, Sprite>();
                foreach (var sprite in sprites)
                {
                    item.Add(id, sprite);
                    id++;
                }
                items.Add(itemType, item);
            }
            _items = items;

            var projectiles = new Dictionary<ActorId, IReadOnlyDictionary<int, Sprite>>();
            foreach (var (actorId, sprites) in projectileSprites)
            {
                var id = 1;
                var projectile = new Dictionary<int, Sprite>();
                foreach (var sprite in sprites)
                {
                    projectile.Add(id, sprite);
                    id++;
                }
                projectiles.Add(actorId, projectile);
            }
            _projectiles = projectiles;
        }

        public Sprite GetItem(ItemType itemType, int id)
        {
            if (!_items.TryGetValue(itemType, out var sprites)) return _none;

            return sprites.GetValueOrDefault(id, _none);
        }

        public Sprite GetProjectile(ActorId actorId, int id)
        {
            if (!_projectiles.TryGetValue(actorId, out var sprites)) return _none;

            return sprites.GetValueOrDefault(id, _none);
        }
    }
}