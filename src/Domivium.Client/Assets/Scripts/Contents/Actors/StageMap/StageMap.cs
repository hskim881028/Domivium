using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Domivium.Client.Contents.Actors.Contract;
using Domivium.Client.Core.Actors;
using Domivium.Client.Core.Actors.Contract;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Domivium.Client.Contents.Actors
{
    public class StageMap : Actor
    {
        [SerializeField] private Tilemap _background;
        [SerializeField] private Tilemap _grid;
        [SerializeField] private Tilemap _preview;
        [SerializeField] private TileBase _backgroundBase;
        [SerializeField] private TileBase _gridBase;
        [SerializeField] private NavMeshSurface _navMeshSurface;

        private readonly HashSet<Vector3Int> _tower = new();
        public Tilemap Background => _background;

        public override UniTask ActivateAsync(CancellationToken token, ActorParam param)
        {
            var bounds = param.As<StageMapParams>().CellBounds;
            _navMeshSurface.size = new Vector3(bounds.size.x - 1, 1, bounds.size.y - 1);
            foreach (var cell in bounds.allPositionsWithin)
            {
                _background.SetTile(cell, _backgroundBase);
                _grid.SetTile(cell, _gridBase);
                _grid.SetColor(cell, Color.gray);
                _preview.SetTile(cell, _gridBase);
                _preview.SetColor(cell, Color.clear);
            }

            return base.ActivateAsync(token, param);
        }

        public void SetActivePreviewGrid(bool value)
        {
            _grid.gameObject.SetActive(value);
            _preview.gameObject.SetActive(value);
        }

        public void BuildNavMesh() => _navMeshSurface.BuildNavMesh();

        public void DrawPreview(Vector3Int cell, bool canPlace)
        {
            _tower.Add(cell);
            _preview.SetColor(cell, canPlace ? Color.greenYellow : Color.indianRed);
        }

        public void ResetPreview()
        {
            foreach (var cell in _tower)
            {
                _preview.SetColor(cell, Color.clear);
            }

            _tower.Clear();
        }

        public void Placement(Vector3Int cell)
        {
            _grid.SetColor(cell, Color.black);
        }
    }
}