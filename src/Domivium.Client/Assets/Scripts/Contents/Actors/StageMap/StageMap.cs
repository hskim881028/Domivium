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
        [SerializeField] private TileBase _gridBase;
        [SerializeField] private TileBase _towerBase;
        [SerializeField] private NavMeshSurface _navMeshSurface;

        private readonly HashSet<Vector3Int> _tower = new();

        public Tilemap Background => _background;

        public override UniTask ActivateAsync(CancellationToken token, ActorParam param, bool immediately = false)
        {
            foreach (var cell in param.As<ActorParams>().Cells)
            {
                _background.SetTile(cell, _gridBase);
                _grid.SetTile(cell, _gridBase);
                _grid.SetColor(cell, Color.gray);
                _preview.SetTile(cell, _gridBase);
                _preview.SetColor(cell, Color.clear);
            }

            _navMeshSurface.BuildNavMesh();
            return base.ActivateAsync(token, param, immediately);
        }

        public void SetActivePreviewGrid(bool value)
        {
            _grid.gameObject.SetActive(value);
            _preview.gameObject.SetActive(value);
        }

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
            _background.SetTile(cell, _towerBase);
            _grid.SetColor(cell, Color.black);
        }
    }
}