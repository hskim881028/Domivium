using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Domivium.Client.Contents.Commands
{
    public interface IBattleUserCommand
    {
        public UniTask InitializeAsync(int stageId);
        public bool PickCharacter(Vector2 position);
        public bool UpdateMoveTarget(Vector2 position);
        public bool SelectCharacter(Vector2 position);
    }
}