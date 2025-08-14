using UnityEngine;
using VContainer;

namespace Domivium.Client.Core.Actor
{
    public class EnvironmentRig : Actor
    {
        public Light DirectionalLight { get; private set; }

        [Inject]
        public void Construct()
        {
            DirectionalLight = GetComponentInChildren<Light>();
        }
    }
}