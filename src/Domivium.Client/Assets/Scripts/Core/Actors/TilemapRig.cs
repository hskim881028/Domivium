using UnityEngine.Tilemaps;
using VContainer;

namespace Domivium.Client.Core.Actors
{
    public sealed class TilemapRig : Actor
    {
        public Tilemap Background { get; private set; }

        public Tilemap Build { get; private set; }

        public Tilemap Preview { get; private set; }

        [Inject]
        public void Construct()
        {
            var tilemaps = GetComponentsInChildren<Tilemap>();
            foreach (var tilemap in tilemaps)
            {
                var order = tilemap.GetComponent<TilemapRenderer>().sortingOrder;
                switch (order)
                {
                    case 0:
                        Background = tilemap;
                        break;
                    case 1:
                        Build = tilemap;
                        break;
                    case 2:
                        Preview = tilemap;
                        break;
                }
            }
        }
    }
}