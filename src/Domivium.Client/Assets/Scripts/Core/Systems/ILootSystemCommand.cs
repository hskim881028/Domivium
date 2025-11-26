using System.Collections.Generic;
using UnityEngine;

namespace Domivium.Client.Core.Systems
{
    public interface ILootSystemCommand : ITicker
    {
        public void Initialize(IReadOnlyDictionary<ushort, Vector2> stageProps);
    }
}