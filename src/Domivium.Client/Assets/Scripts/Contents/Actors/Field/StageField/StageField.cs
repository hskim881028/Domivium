using System.Collections.Generic;
using Domivium.Client.Contents.Components;
using Domivium.Client.Data.Loot;
using UnityEngine;

namespace Domivium.Client.Contents.Actors
{
    public class StageField : Field
    {
        private readonly Dictionary<LootType, IReadOnlyList<(ushort lootId, Vector2 position)>> _loots = new();

        public IReadOnlyDictionary<LootType, IReadOnlyList<(ushort lootId, Vector2 position)>> Loots => _loots;

        protected override void OnAwake()
        {
            base.OnAwake();
            var lootEntities = new Dictionary<LootType, List<(ushort lootId, Vector2 position)>>();
            var props = transform.GetComponentsInChildren<LootProp>();
            foreach (var prop in props)
            {
                if (!lootEntities.ContainsKey(prop.LootType))
                {
                    lootEntities.Add(prop.LootType, new List<(ushort lootId, Vector2 position)>());
                }

                lootEntities[prop.LootType].Add((prop.LootId, prop.transform.position));
            }

            foreach (var (lootType, loots) in lootEntities)
            {
                _loots.Add(lootType, loots);
            }
        }
    }
}