using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Domivium.Client.Contents.DI.Container
{
    [CreateAssetMenu(fileName = "StageFieldContainer", menuName = "ScriptableObjects/StageFieldContainer")]
    public class StageFieldContainer : ScriptableObject
    {
        [SerializeField] private List<Tilemap> _biome;

        public List<Tilemap> Biome => _biome;
    }
}