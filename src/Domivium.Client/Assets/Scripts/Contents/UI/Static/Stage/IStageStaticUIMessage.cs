using Domivium.Client.Core.UI;
using UnityEngine;

namespace Domivium.Client.Contents.UI.Static
{
    public interface IStageStaticUIMessage : IUIMessage
    {
        public void EnterLobby();
        public void SelectTower(int index);
    }
}