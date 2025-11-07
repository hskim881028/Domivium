using UnityEngine;

namespace Domivium.Client.Core.Systems
{
    public interface ICameraSystemCommand
    {
        public void Initialize(Transform character);
    }
}