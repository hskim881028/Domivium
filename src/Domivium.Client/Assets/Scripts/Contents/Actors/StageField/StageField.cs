using System.Collections.Generic;
using Domivium.Client.Contents.Components;
using Domivium.Client.Core.Actors;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Domivium.Client.Contents.Actors
{
    public class StageField : Actor
    {
        [SerializeField] private Tilemap _colliderGrid;

        private readonly Dictionary<ushort, Vector2> _stageProps = new();

        protected override void OnAwake()
        {
            base.OnAwake();
            var props = transform.GetComponentsInChildren<StageProp>();
            foreach (var prop in props)
            {
                _stageProps.Add(prop.LootId, prop.transform.position);
            }
        }

        public Tilemap ColliderGrid => _colliderGrid;
        public IReadOnlyDictionary<ushort, Vector2> StageProps => _stageProps;
    }
}