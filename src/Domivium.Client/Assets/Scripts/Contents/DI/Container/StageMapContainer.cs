using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Domivium.Client.Contents.DI.Container
{
    [CreateAssetMenu(fileName = "StageMapContainer", menuName = "ScriptableObjects/StageMapContainer")]
    public class StageMapContainer : ScriptableObject
    {
        [SerializeField] private List<Tilemap> _biome;

        public List<Tilemap> Biome => _biome;
    }
}