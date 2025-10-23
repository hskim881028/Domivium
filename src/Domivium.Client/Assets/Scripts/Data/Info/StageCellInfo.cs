using Domivium.Client.Data.StageField;
using UnityEngine;

namespace Domivium.Client.Data.Info
{
    public readonly struct StageCellInfo
    {
        public Vector3Int Cell { get; }
        public StageCellTag Tag { get; }

        private StageCellInfo(Vector3Int cell, StageCellTag tag)
        {
            Cell = cell;
            Tag = tag;
        }

        public static StageCellInfo Create(Vector3Int cell, StageCellTag tag) => new(cell, tag);

        public static StageCellInfo Empty => new(Vector3Int.zero, new StageCellTag());
    }
}