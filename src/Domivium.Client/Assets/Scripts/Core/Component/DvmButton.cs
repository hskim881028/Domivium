using Domivium.Client.Core.UI.View;
using TMPro;
using UnityEngine.UI;

namespace Domivium.Client.Core.Component
{
    public class DvmButton : Button, IUISelectable
    {
        private Image _image;
        private TextMeshProUGUI _text;

        public UIViewBase Parent { get; private set; }

        public string Text
        {
            get => _text.text;
            set => _text.text = value;
        }

        protected override void Awake()
        {
            base.Awake();
            _image = GetComponent<Image>();
            _text = GetComponentInChildren<TextMeshProUGUI>();
            Parent = GetComponentInParent<UIViewBase>();
        }
    }
}