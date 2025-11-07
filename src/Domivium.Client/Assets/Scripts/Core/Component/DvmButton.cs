using Domivium.Client.Core.UI.View;
using UnityEngine.UI;

namespace Domivium.Client.Core.Component
{
    public class DvmButton : Button, IUISelectable
    {
        public UIBehaviour Parent { get; private set; }

        protected override void Awake()
        {
            base.Awake();
            Parent = GetComponentInParent<UIBehaviour>();
        }
    }
}