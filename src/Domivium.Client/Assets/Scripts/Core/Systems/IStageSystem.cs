using UnityEngine;

namespace Domivium.Client.Core.Systems
{
    public interface IStageSystem
    {
        public Vector3 GetNextPosition(Vector3 position, Vector3 delta, Vector2 collider);
    }
}