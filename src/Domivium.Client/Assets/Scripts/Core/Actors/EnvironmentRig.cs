using UnityEngine;
using VContainer;

namespace Domivium.Client.Core.Actors
{
    public sealed class EnvironmentRig : Actor
    {
        public Light DirectionalLight { get; private set; }

        [Inject]
        public void Construct()
        {
            DirectionalLight = GetComponentInChildren<Light>();
        }
    }
}