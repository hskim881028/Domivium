using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Domivium.Client.Contents.Actors.Contract;
using Domivium.Client.Core.Actors;
using Domivium.Client.Core.Actors.Contract;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Domivium.Client.Contents.Actors.StageMap
{
    public class StageMap : Actor
    {
        [SerializeField] private Tilemap _background;
        [SerializeField] private Tilemap _grid;
        [SerializeField] private Tilemap _preview;
        [SerializeField] private TileBase _base;

        private readonly HashSet<Vector3Int> _tower = new();

        public Tilemap Background => _background;

        public override void Initialize(Transform parent, Action onDespawn)
        {
            base.Initialize(parent, onDespawn);
        }

        public override UniTask ShowAsync(CancellationToken token, ActorParam param, bool immediately = false)
        {
            foreach (var cell in param.As<StageMapParam>().Cells)
            {
                _background.SetTile(cell, _base);
                _background.SetColor(cell, Color.mediumSeaGreen);
                _grid.SetTile(cell, _base);
                _grid.SetColor(cell, Color.gray);
                _preview.SetTile(cell, _base);
                _preview.SetColor(cell, Color.clear);
            }

            return base.ShowAsync(token, param, immediately);
        }

        public override UniTask HideAsync(CancellationToken token, bool immediately = false) => base.HideAsync(token, immediately);

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
            _background.SetColor(cell, Color.blueViolet);
            _grid.SetColor(cell, Color.black);
        }
    }
}