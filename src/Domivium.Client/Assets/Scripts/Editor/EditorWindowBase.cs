using System;
using UnityEditor;
using UnityEngine;

namespace Domivium.Client.Editor
{
    public abstract class EditorWindowBase : EditorWindow
    {
        private Texture2D _normalTex;
        private Texture2D _hoverTex;
        private Texture2D _activeTex;
        private bool _stylesInitialized;

        protected GUIStyle ButtonStyle;
        protected abstract void OnEnableInternal();
        protected abstract void OnDisableInternal();
        protected abstract void OnGUIInternal();

        private void OnEnable()
        {
            OnEnableInternal();
            SetStyles();
        }

        private void OnDisable()
        {
            OnDisableInternal();

            if (_normalTex) DestroyImmediate(_normalTex);
            if (_hoverTex) DestroyImmediate(_hoverTex);
            if (_activeTex) DestroyImmediate(_activeTex);

            _stylesInitialized = false;
            ButtonStyle = null;
        }

        private void OnGUI()
        {
            SetStyles();
            OnGUIInternal();
        }

        private void SetStyles()
        {
            if (_stylesInitialized) return;

            try
            {
                _normalTex = CreateColorTexture(new Color(0.23f, 0.35f, 0.48f));
                _hoverTex = CreateColorTexture(new Color(0.28f, 0.45f, 0.65f));
                _activeTex = CreateColorTexture(new Color(0.2f, 0.3f, 0.4f));

                ButtonStyle = new GUIStyle(GUI.skin.button)
                {
                    fontSize = 12,
                    padding = new RectOffset(15, 15, 5, 5),
                    margin = new RectOffset(5, 5, 5, 5),
                    normal = { background = _normalTex, textColor = new Color(0.9f, 0.9f, 0.9f) },
                    hover = { background = _hoverTex, textColor = Color.white },
                    active = { background = _activeTex, textColor = Color.white }
                };

                _stylesInitialized = true;
            }
            catch (Exception)
            {
                _stylesInitialized = false;
            }
        }

        private Texture2D CreateColorTexture(Color color)
        {
            var texture = new Texture2D(1, 1)
            {
                hideFlags = HideFlags.HideAndDontSave
            };
            texture.SetPixel(0, 0, color);
            texture.Apply();
            return texture;
        }
    }
}