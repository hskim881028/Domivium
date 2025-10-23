using System.Threading;
using Cysharp.Threading.Tasks;
using Domivium.Client.Contents.Actors.Contract;
using Domivium.Client.Core.Actors;
using Domivium.Client.Core.Actors.Contract;
using Domivium.Client.Data.StageField;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Domivium.Client.Contents.Actors
{
    public class StageField : Actor
    {
        [SerializeField] private Tilemap _background;
        [SerializeField] private Tilemap _grid;
        [SerializeField] private Tilemap _preview;
        [SerializeField] private TileBase _backgroundBase;
        [SerializeField] private TileBase _gridBase;
        [SerializeField] private NavMeshSurface _navMeshSurface;

        private Vector3Int _cell;

        public Tilemap Background => _background;

        public override UniTask ActivateAsync(CancellationToken token, ActorParam param)
        {
            var bounds = param.As<StageFieldParams>().CellBounds;
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

        public void DrawPreview(Vector3Int cell, StageCellTag cellTag)
        {
            this.Log(cellTag);
            _preview.SetColor(_cell, Color.clear);
            _cell = cell;

            _preview.SetColor(_cell, GetPreviewColor(cellTag));
        }

        public void Placement(Vector3Int cell)
        {
            _grid.SetColor(cell, Color.black);
        }

        public void Release(Vector3Int cell)
        {
            _grid.SetColor(cell, Color.gray);
        }

        private Color GetPreviewColor(StageCellTag cellTag)
        {
            if (cellTag == StageCellTag.Occupiable) return Color.greenYellow;

            if (cellTag == StageCellTag.Upgradeable) return Color.deepSkyBlue;

            return Color.indianRed;
        }
    }
}