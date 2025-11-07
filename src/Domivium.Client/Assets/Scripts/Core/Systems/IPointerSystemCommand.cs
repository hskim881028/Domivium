using UnityEngine;

namespace Domivium.Client.Core.Systems
{
    public interface IPointerSystemCommand
    {
        public void Update(Vector2 pointer);
    }
}