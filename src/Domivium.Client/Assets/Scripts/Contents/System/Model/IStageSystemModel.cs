using UnityEngine;

namespace Domivium.Client.Contents.System.Model
{
    public interface IStageSystemModel
    {
        public Vector3 NextPosition(Vector3 position, Vector3 delta, Vector2 collider);
    }
}