using UnityEngine;

namespace Domivium.Client.Core.Systems
{
    public interface ICharacterSystemCommand
    {
        public void Stop();
        public bool SetDirection(Vector2 value);
        public bool LookAt(Vector2 value);
        public bool Avoid();
    }
}