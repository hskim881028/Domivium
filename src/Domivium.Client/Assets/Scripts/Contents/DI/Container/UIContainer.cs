using System.Collections.Generic;
using Domivium.Client.Core.UI.View;
using UnityEngine;

namespace Domivium.Client.Contents.DI.Container
{
    [CreateAssetMenu(fileName = "UIContainer", menuName = "ScriptableObjects/UIContainer")]
    public class UIContainer : ScriptableObject
    {
        [SerializeField] private List<UIBehaviour> _ui = new();

        public List<UIBehaviour> UI => _ui;
    }
}