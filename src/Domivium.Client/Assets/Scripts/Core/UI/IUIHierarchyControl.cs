using UnityEngine;

namespace Domivium.Client.Core.UI
{
    public interface IUIHierarchyControl
    {
        public void SetParent(Transform parent);
    }
}