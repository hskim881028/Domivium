using Domivium.Client.Core.Systems;
using UnityEngine;

namespace Domivium.Client.Contents.Systems
{
    public sealed class PointerSystem : IPointerSystem, IPointerSystemCommand
    {
        public Vector2 Current { get; private set; }

        public void Update(Vector2 pointer)
        {
            Current = pointer;
        }
    }
}