using UnityEngine;

namespace Domivium.Client.Core.Actors
{
    public interface IAgent
    {
        public void SetSpeed(int speed);
        public void SetTargetPosition(Vector3 position);
    }
}