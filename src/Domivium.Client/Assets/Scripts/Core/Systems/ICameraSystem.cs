using UnityEngine;

namespace Domivium.Client.Core.Systems
{
    public interface ICameraSystem
    {
        public Camera MainCamera { get; }
        public Camera UICamera { get; }
    }
}