using Domivium.Client.Core.Systems;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Domivium.Client.Contents.Systems
{
    public sealed class StageFieldSystem : Disposable, IStageFieldSystem, IStageFieldSystemCommand
    {
        private const float Skin = 0.01f;
        private const float BiasInX = 0.002f;
        private const float BiasInY = 0.002f;
        private const float LeadOut = 0.003f;
        private const int IterationCount = 10;

        private Tilemap _grid;

        public void InitializeAsync(Tilemap grid)
        {
            _grid = grid;
        }

        public Vector3 GetNextPosition(Vector3 position, Vector3 delta, Vector2 collider)
        {
            var target = position + delta;
            if (IsValid(target, delta, collider)) return target;

            var first = Mathf.Abs(delta.x) >= Mathf.Abs(delta.y);
            var nextPosition = position;

            if (first)
            {
                Vector3 dx = new(delta.x, 0f, 0f);
                nextPosition += dx * Sweep(nextPosition, dx, collider);

                Vector3 dy = new(0f, delta.y, 0f);
                nextPosition += dy * Sweep(nextPosition, dy, collider);
            }
            else
            {
                Vector3 dy = new(0f, delta.y, 0f);
                nextPosition += dy * Sweep(nextPosition, dy, collider);

                Vector3 dx = new(delta.x, 0f, 0f);
                nextPosition += dx * Sweep(nextPosition, dx, collider);
            }

            return nextPosition;
        }

        private bool HasTile(Vector3 p)
        {
            var cell = _grid.WorldToCell(p);
            return _grid.cellBounds.Contains(cell) && _grid.HasTile(cell);
        }

        private bool IsValid(Vector3 position, Vector3 delta, Vector2 collider)
        {
            var hx = Mathf.Max(0f, collider.x - Skin);
            var hy = Mathf.Max(0f, collider.y - Skin);
            var leftX = position.x - hx + BiasInX;
            var rightX = position.x + hx - BiasInX;
            var botY = position.y + BiasInY;
            var topY = position.y + hy - BiasInY;

            switch (delta.y)
            {
                case > 0f:
                    topY = position.y + hy + LeadOut;
                    break;
                case < 0f:
                    botY = position.y - LeadOut;
                    break;
            }

            switch (delta.x)
            {
                case < 0f:
                    leftX = position.x - hx - LeadOut;
                    break;
                case > 0f:
                    rightX = position.x + hx + LeadOut;
                    break;
            }

            var bl = new Vector3(leftX, botY);
            var br = new Vector3(rightX, botY);
            var tl = new Vector3(leftX, topY);
            var tr = new Vector3(rightX, topY);
            return HasTile(bl) && HasTile(br) && HasTile(tl) && HasTile(tr);
        }

        private float Sweep(Vector3 start, Vector3 delta, Vector2 collider)
        {
            float lo = 0f, hi = 1f;
            for (var i = 0; i < IterationCount; i++)
            {
                var mid = (lo + hi) * 0.5f;
                var position = start + delta * mid;
                if (IsValid(position, delta, collider))
                {
                    lo = mid;
                }
                else
                {
                    hi = mid;
                }
            }
            return Mathf.Max(0f, lo - Skin * 0.5f);
        }
    }
}