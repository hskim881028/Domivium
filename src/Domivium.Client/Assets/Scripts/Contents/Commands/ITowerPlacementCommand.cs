using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Domivium.Client.Contents.Commands
{
    public interface ITowerPlacementCommand
    {
        public UniTask InitializeAsync(int stageId);
        public void Show(int index);
        public bool Hide();
        public bool Update(Vector2 position);
        public bool Placement(Vector2 position);
    }
}