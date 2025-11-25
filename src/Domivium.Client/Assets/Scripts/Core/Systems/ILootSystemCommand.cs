using System.Collections.Generic;
using UnityEngine;

namespace Domivium.Client.Core.Systems
{
    public interface ILootSystemCommand
    {
        public void Initialize(Transform character, IReadOnlyDictionary<ushort, Vector2> stageProps);
        public void Tick();
    }
}