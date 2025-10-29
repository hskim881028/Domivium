using Domivium.Client.Contents.System.Command;
using Domivium.Client.Contents.System.Model;
using UnityEngine;

namespace Domivium.Client.Contents.System
{
    public sealed class PointerSystem : IPointerSystemModel, IPointerSystemCommand
    {
        public Vector2 Current { get; private set; }

        public void Update(Vector2 pointer)
        {
            Current = pointer;
        }
    }
}