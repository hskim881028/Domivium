using Domivium.Client.Data.Item;
using UnityEngine;

namespace Domivium.Client.Core.Systems
{
    public interface ISpriteSystem
    {
        public Sprite GetSprite(ItemType itemType, int id);
    }
}