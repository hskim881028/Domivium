using Domivium.Client.Core.Battle;
using UnityEngine;

namespace Domivium.Client.Core.Systems
{
    public interface ICharacterSystemCommand
    {
        public void Initialize(IBattleSystem character);
        public void Stop();
        public bool SetDirection(Vector2 value);
        public bool LookAt(Vector2 value);
        public bool Avoid();
    }
}