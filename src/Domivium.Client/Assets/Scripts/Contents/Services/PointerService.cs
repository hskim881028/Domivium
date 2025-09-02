using Domivium.Client.Contents.Commands;
using Domivium.Client.Contents.ReadModels;
using UnityEngine;

namespace Domivium.Client.Contents.Services
{
    public sealed class PointerService : IPointerReadModel, IPointerCommand
    {
        public Vector2 Current { get; private set; }

        public void Update(Vector2 pointer)
        {
            Current = pointer;
        }
    }
}