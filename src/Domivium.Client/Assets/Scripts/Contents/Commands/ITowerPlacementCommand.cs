using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Domivium.Client.Contents.Commands
{
    public interface ITowerPlacementCommand
    {
        public IReadOnlyCollection<Vector3Int> Initialize(int stageId);
        public void SetGrid(Tilemap tilemap);
        public void Show(int index);
        public bool Hide();
        public bool Update(Vector2 position);
        public bool Placement(Vector2 position);
    }
}