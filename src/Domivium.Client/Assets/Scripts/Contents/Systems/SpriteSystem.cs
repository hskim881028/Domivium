using System.Collections.Generic;
using System.Linq;
using Domivium.Client.Core.Systems;
using Domivium.Client.Data.Item;
using UnityEngine;

namespace Domivium.Client.Contents.Systems
{
    public class SpriteSystem : ISpriteSystem
    {
        private readonly Sprite _none;
        private readonly IReadOnlyDictionary<ItemType, IReadOnlyDictionary<int, Sprite>> _items;

        public SpriteSystem(Dictionary<ItemType, List<Sprite>> itemSprites)
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
        }

        public Sprite GetSprite(ItemType itemType, int id)
        {
            if (!_items.TryGetValue(itemType, out var sprites))
            {
                return _none;
            }

            return sprites.GetValueOrDefault(id, _none);
        }
    }
}