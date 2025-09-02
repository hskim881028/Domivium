using UnityEngine;

namespace Domivium.Client.Contents.Commands
{
    public interface IPointerCommand
    {
        public void Update(Vector2 pointer);
    }
}