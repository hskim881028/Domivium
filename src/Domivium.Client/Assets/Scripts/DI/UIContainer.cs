using System.Collections.Generic;
using Domivium.Client.Core.UI.View;
using UnityEngine;

namespace Domivium.Client.DI
{
    [CreateAssetMenu(fileName = "UIContainer", menuName = "ScriptableObjects/UIContainer")]
    public class UIContainer : ScriptableObject
    {
        [SerializeField] private List<UIViewBase> _ui = new();

        public List<UIViewBase> UI => _ui;
    }
}