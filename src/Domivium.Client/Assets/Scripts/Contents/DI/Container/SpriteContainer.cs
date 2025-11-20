using System.Collections.Generic;
using Domivium.Client.Data.Item;
using UnityEngine;

namespace Domivium.Client.Contents.DI.Container
{
    [CreateAssetMenu(fileName = "SpriteContainer", menuName = "ScriptableObjects/SpriteContainer")]
    public class SpriteContainer : ScriptableObject
    {
        [SerializeField] private Sprite _none;
        [SerializeField] private List<Sprite> _weapon = new();
        [SerializeField] private List<Sprite> _projectile = new();

        public Dictionary<ItemType, List<Sprite>> Items()
        {
            return new Dictionary<ItemType, List<Sprite>>
            {
                { ItemType.None, new List<Sprite>() { _none } },
                { ItemType.Weapon, _weapon },
                { ItemType.Projectile, _projectile }
            };
        }
    }
}