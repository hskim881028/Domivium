using Domivium.Client.Core.Actors;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Domivium.Client.Contents.Actors
{
    public class StageField : Actor
    {
        [SerializeField] private Tilemap _colliderGrid;

        public Tilemap ColliderGrid => _colliderGrid;
    }
}