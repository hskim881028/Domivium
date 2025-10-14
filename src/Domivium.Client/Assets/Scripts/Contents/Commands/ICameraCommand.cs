using UnityEngine;

namespace Domivium.Client.Contents.Commands
{
    public interface ICameraCommand
    {
        public void Initialize(int stageId);
        public bool MoveStarted(Vector2 position);
        public bool UpdatePosition(Vector2 position);
        public bool MoveEnd(Vector2 position);
    }
}