using Domivium.Client.Core.Actors;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Domivium.Client.Contents.Actors
{
    public class Field : Actor
    {
        [SerializeField] private Tilemap _colliderGrid;

        public Tilemap ColliderGrid => _colliderGrid;
    }
}