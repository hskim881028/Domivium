using UnityEngine;

namespace Domivium.Client.Contents.System.Command
{
    public interface IPointerSystemCommand
    {
        public void Update(Vector2 pointer);
    }
}