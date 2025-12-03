using System.Collections.Generic;
using Domivium.Client.Core.Actors;
using Domivium.Client.Data.Item;
using UnityEngine;

namespace Domivium.Client.Contents.DI.Container
{
    [CreateAssetMenu(fileName = "SpriteContainer", menuName = "ScriptableObjects/SpriteContainer")]
    public class SpriteContainer : ScriptableObject
    {
        [SerializeField] private Sprite _noneItem;
        [SerializeField] private List<Sprite> _weaponItems = new();
        [SerializeField] private List<Sprite> _projectileItems = new();
        [SerializeField] private List<Sprite> _ringItems = new();
        [SerializeField] private List<Sprite> _necklaceItems = new();
        [SerializeField] private List<Sprite> _headItems = new();
        [SerializeField] private List<Sprite> _bodyItems = new();
        [SerializeField] private List<Sprite> _feetItems = new();
        [SerializeField] private List<Sprite> _bagItems = new();
        [SerializeField] private List<Sprite> _potionItems = new();
        [SerializeField] private List<Sprite> _foodItems = new();
        [SerializeField] private List<Sprite> _cashItems = new();
        [SerializeField] private List<Sprite> _materialItems = new();

        [SerializeField] private List<Sprite> _projectiles = new();
        [SerializeField] private List<Sprite> _monsterProjectiles = new();

        public Dictionary<ItemType, List<Sprite>> Items => new()
        {
            { ItemType.None, new List<Sprite> { _noneItem } },
            { ItemType.Weapon, _weaponItems },
            { ItemType.Projectile, _projectileItems },
            { ItemType.Ring, _ringItems },
            { ItemType.Necklace, _necklaceItems },
            { ItemType.Head, _headItems },
            { ItemType.Body, _bodyItems },
            { ItemType.Feet, _feetItems },
            { ItemType.Bag, _bagItems },
            { ItemType.Potion, _potionItems },
            { ItemType.Food, _foodItems },
            { ItemType.Cash, _cashItems },
            { ItemType.Material, _materialItems }
        };

        public Dictionary<ActorId, List<Sprite>> Projectiles => new()
        {
            { ActorId.Character, _projectiles },
            { ActorId.Monster, _monsterProjectiles }
        };
    }
}