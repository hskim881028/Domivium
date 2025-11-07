using System.Threading;
using Cysharp.Threading.Tasks;
using Domivium.Client.Contents.Actors.Contract;
using Domivium.Client.Core.Actors;
using Domivium.Client.Core.Actors.Contract;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Domivium.Client.Contents.Actors
{
    public class StageField : Actor
    {
        [SerializeField] private Tilemap _background;
        [SerializeField] private Tilemap _colliderGrid;
        [SerializeField] private Tilemap _debug;
        [SerializeField] private TileBase _backgroundBase;
        [SerializeField] private TileBase _treeTile;
        [SerializeField] private Transform _propContainer;

        private Vector3Int _cell;

        public Tilemap ColliderGrid => _colliderGrid;

        public override async UniTask SpawnAsync(CancellationToken token, ActorParam param)
        {
            await base.SpawnAsync(token, param);

            var tilemap = param.As<StageFieldParams>().Tilemap;

            foreach (var cell in tilemap.cellBounds.allPositionsWithin)
            {
                _background.SetTile(cell, _backgroundBase);

                if (tilemap.HasTile(cell))
                {
                    _colliderGrid.SetTile(cell, _backgroundBase);
                }
                else
                {
                    _debug.SetTile(cell, _treeTile);
                }
            }
        }
    }
}