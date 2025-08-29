using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Domivium.Client.Contents.DI.Container;
using Domivium.Client.Core.UI;
using Domivium.Client.Core.UI.View;
using Domivium.Client.Core.Utility;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Domivium.Client.Editor
{
    public class UIEditorWindow : EditorWindow
    {
        private static UIType _uiType;
        private static Object _uiContainer;
        private static string _uiName = string.Empty;

        private GUIStyle _buttonStyle;
        private Texture2D _normalTex;
        private Texture2D _hoverTex;
        private Texture2D _activeTex;
        private bool _stylesInitialized;

        static UIEditorWindow()
        {
            AssemblyReloadEvents.afterAssemblyReload += OnAfterAssemblyReload;
        }

        [MenuItem("Domivium/UI Editor")]
        public static void ShowWindow()
        {
            Refresh();
            EditorSceneManager.OpenScene(EditorConfig.Workspace);
            GetWindow<UIEditorWindow>("UI Editor");
        }

        private void OnEnable()
        {
            _uiType = (UIType)SessionState.GetInt(EditorConfig.TypeSessionKey, (int)_uiType);
            _uiName = SessionState.GetString(EditorConfig.NameSessionKey, _uiName);

            Refresh();
            SetStyles();
        }

        private void OnDisable()
        {
            SessionState.SetInt(EditorConfig.TypeSessionKey, (int)_uiType);
            SessionState.SetString(EditorConfig.NameSessionKey, string.Empty);
            GUI.FocusControl(null);
            EditorGUIUtility.editingTextField = false;
            Repaint();

            if (_normalTex) DestroyImmediate(_normalTex);
            if (_hoverTex) DestroyImmediate(_hoverTex);
            if (_activeTex) DestroyImmediate(_activeTex);

            _stylesInitialized = false;
            _buttonStyle = null;
        }

        private void OnGUI()
        {
            SetStyles();

            DrawContainerField();
            DrawUITypeSelection();
            DrawUINameField();
            DrawButtons();
        }

        private static void OnAfterAssemblyReload()
        {
            var selectedUIType = EditorPrefs.GetString(EditorConfig.TypePrefsKey, string.Empty);
            if (!Enum.TryParse<UIType>(selectedUIType, out var uiType)) return;

            var uiName = EditorPrefs.GetString(EditorConfig.NamePrefsKey, string.Empty);
            if (string.IsNullOrEmpty(uiName)) return;

            EditorApplication.delayCall += () =>
            {
                try
                {
                    CreatePrefabWithView(uiType, uiName);
                    RefreshSettings();
                    DisplayDialog($"{uiType} -> {uiName}");
                }
                catch (Exception ex)
                {
                    DisplayDialog(ex.Message, true);
                }
                finally
                {
                    EditorPrefs.DeleteKey(EditorConfig.TypePrefsKey);
                    EditorPrefs.DeleteKey(EditorConfig.NamePrefsKey);
                }
            };
        }

        private static void RefreshSettings()
        {
            GenerateUIIds();
            GenerateUIMapping();
            UpdateUIContainer();
        }

        private static void Refresh()
        {
            _uiContainer = AssetDatabase.LoadAssetAtPath<UIContainer>(EditorConfig.UIContainer);
        }

        private void SetStyles()
        {
            if (_stylesInitialized) return;

            try
            {
                _normalTex = CreateColorTexture(new Color(0.23f, 0.35f, 0.48f));
                _hoverTex = CreateColorTexture(new Color(0.28f, 0.45f, 0.65f));
                _activeTex = CreateColorTexture(new Color(0.2f, 0.3f, 0.4f));

                _buttonStyle = new GUIStyle(GUI.skin.button)
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

        private static void DrawContainerField()
        {
            _uiContainer = EditorGUILayout.ObjectField("Container", _uiContainer, typeof(UIContainer), false);
        }

        private void DrawUITypeSelection()
        {
            EditorGUI.BeginChangeCheck();

            var uiTypes = Enum.GetValues(typeof(UIType)).Cast<UIType>().ToArray();
            var newUIType = (UIType)EditorGUILayout.IntPopup("Type",
                (int)_uiType,
                uiTypes.Select(t => t.ToString()).ToArray(),
                uiTypes.Select(t => (int)t).ToArray());

            if (EditorGUI.EndChangeCheck())
            {
                _uiType = newUIType;
            }
        }

        private void DrawUINameField()
        {
            EditorGUI.BeginChangeCheck();
            var newUIName = EditorGUILayout.TextField("Name", _uiName);
            if (EditorGUI.EndChangeCheck())
            {
                _uiName = CapitalizeFirstLetter(newUIName);
            }
        }

        private void DrawButtons()
        {
            EditorGUILayout.Space(10);
            using (new EditorGUI.DisabledScope(false))
            {
                if (GUILayout.Button("Refresh", _buttonStyle))
                {
                    RefreshSettings();
                }

                if (GUILayout.Button("Generate", _buttonStyle))
                {
                    GenerateUI();
                }
            }
        }

        private static string CapitalizeFirstLetter(string text)
        {
            if (string.IsNullOrEmpty(text)) return text;

            return char.ToUpper(text[0]) + text.Substring(1);
        }

        private static string SanitizeName(string name)
        {
            if (string.IsNullOrWhiteSpace(name)) return string.Empty;

            var s = name.Trim();
            s = Regex.Replace(s, "[^A-Za-z0-9_]", string.Empty);
            if (s.Length == 0) return s;

            return char.ToUpper(s[0]) + (s.Length > 1 ? s.Substring(1) : string.Empty);
        }

        private void GenerateUI()
        {
            _uiName = SanitizeName(_uiName);
            if (string.IsNullOrEmpty(_uiName))
            {
                DisplayDialog("Name is empty or invalid.", true);
                return;
            }

            try
            {
                if (HasDuplicates(_uiType, _uiName)) return;

                var tMsg = LoadTemplate("Message", _uiType);
                var tView = LoadTemplate("View", _uiType);
                var tPresenter = LoadTemplate("Presenter", _uiType);
                if (tMsg == null || tView == null || tPresenter == null)
                {
                    DisplayDialog($"Template missing.\nMessage: {tMsg != null}\nView: {tView != null}\nPresenter: {tPresenter != null}", true);
                    return;
                }

                EditorPrefs.SetString(EditorConfig.NamePrefsKey, _uiName);
                EditorPrefs.SetString(EditorConfig.TypePrefsKey, _uiType.ToString());

                AssetDatabase.StartAssetEditing();
                try
                {
                    var scriptsFolder = CreateScriptsFolder(_uiType, _uiName);
                    CreateMessageScript(_uiType, _uiName, scriptsFolder, tMsg);
                    CreateViewScript(_uiType, _uiName, scriptsFolder, tView);
                    CreatePresenterScript(_uiType, _uiName, scriptsFolder, tPresenter);
                }
                finally
                {
                    AssetDatabase.StopAssetEditing();
                }

                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
            }
            catch (Exception ex)
            {
                DisplayDialog(ex.Message, true);
            }
        }

        private static bool HasDuplicates(UIType uiType, string uiName)
        {
            var basePath = uiType.ToScriptPath();
            var folder = Path.Combine(basePath, uiName);
            if (Directory.Exists(folder))
            {
                DisplayDialog($"Scripts folder already exists: {folder}", true);
                return true;
            }

            var prefabPath = uiType.ToPrefabPath();
            var fullPrefab = Path.Combine(prefabPath, uiName + EditorConfig.PrefabExtension);
            if (File.Exists(fullPrefab))
            {
                DisplayDialog($"Prefab already exists: {fullPrefab}", true);
                return true;
            }

            return false;
        }

        private static string CreateScriptsFolder(UIType uiType, string uiName)
        {
            var basePath = uiType.ToScriptPath();
            if (!Directory.Exists(basePath)) Directory.CreateDirectory(basePath);
            var folder = Path.Combine(basePath, uiName);
            Directory.CreateDirectory(folder);

            var assetsRel = EditorConfig.ToAssetsRelative(folder);
            AssetDatabase.ImportAsset(assetsRel);
            return folder;
        }

        private static void CreateMessageScript(UIType uiType, string uiName, string folder, string template)
        {
            var ns = EditorConfig.GetContentsNamespaceName(uiType);
            var interfaceName = uiType.ToMessage(uiName);

            var content = FillTemplate(template,
                ("NAMESPACE", ns),
                ("INTERFACE_NAME", interfaceName),
                ("UI_TYPE", uiType.ToString()),
                ("VIEW_NAME", uiType.ToView(uiName)),
                ("PRESENTER_NAME", uiType.ToPresenter(uiName)),
                ("MESSAGE_NAME", interfaceName)
            );
            var outPath = Path.Combine(folder, interfaceName + EditorConfig.CSharpExtension);
            File.WriteAllText(outPath, content, Encoding.UTF8);
            AssetDatabase.ImportAsset(EditorConfig.ToAssetsRelative(outPath));
        }

        private static void CreateViewScript(UIType uiType, string uiName, string folder, string template)
        {
            var ns = EditorConfig.GetContentsNamespaceName(uiType);
            var viewName = uiType.ToView(uiName);
            var messageName = uiType.ToMessage(uiName);

            var content = FillTemplate(template,
                ("NAMESPACE", ns),
                ("UI_TYPE", uiType.ToString()),
                ("VIEW_NAME", viewName),
                ("PRESENTER_NAME", uiType.ToPresenter(uiName)),
                ("MESSAGE_NAME", messageName)
            );
            var outPath = Path.Combine(folder, viewName + EditorConfig.CSharpExtension);
            File.WriteAllText(outPath, content, Encoding.UTF8);
            AssetDatabase.ImportAsset(EditorConfig.ToAssetsRelative(outPath));
        }

        private static void CreatePresenterScript(UIType uiType, string uiName, string folder, string template)
        {
            var ns = EditorConfig.GetContentsNamespaceName(uiType);
            var presenterName = uiType.ToPresenter(uiName);
            var viewName = uiType.ToView(uiName);
            var messageName = uiType.ToMessage(uiName);

            var content = FillTemplate(template,
                ("NAMESPACE", ns),
                ("UI_TYPE", uiType.ToString()),
                ("VIEW_NAME", viewName),
                ("PRESENTER_NAME", presenterName),
                ("MESSAGE_NAME", messageName)
            );
            var outPath = Path.Combine(folder, presenterName + EditorConfig.CSharpExtension);
            File.WriteAllText(outPath, content, Encoding.UTF8);
            AssetDatabase.ImportAsset(EditorConfig.ToAssetsRelative(outPath));
        }

        private static void CreatePrefabWithView(UIType uiType, string uiName)
        {
            var prefabFolder = uiType.ToPrefabPath();
            if (!Directory.Exists(prefabFolder))
            {
                Directory.CreateDirectory(prefabFolder);
            }

            var prefabPath = Path.Combine(prefabFolder, uiName + EditorConfig.PrefabExtension);

            var go = new GameObject(uiName)
            {
                layer = Layer.UI
            };

            go.AddComponent<CanvasRenderer>();
            var rect = go.AddComponent<RectTransform>();
            var canvas = FindFirstObjectByType<Canvas>();
            if (canvas != null)
            {
                rect.SetParent(canvas.transform, false);
            }

            rect.localPosition = Vector3.zero;
            rect.localScale = Vector3.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.pivot = Vector2.one * 0.5f;

            var viewTypeFullName = $"{EditorConfig.GetContentsNamespaceName(uiType)}.{uiType.ToView(uiName)}";
            var viewType = Type.GetType(viewTypeFullName);
            if (viewType == null)
            {
                foreach (var asm in AppDomain.CurrentDomain.GetAssemblies())
                {
                    viewType = asm.GetType(viewTypeFullName);
                    if (viewType != null) break;
                }
            }
            if (viewType != null && typeof(Component).IsAssignableFrom(viewType))
            {
                go.AddComponent(viewType);
            }
            else
            {
                Debug.LogWarning($"View type not found yet: {viewTypeFullName}. Prefab will be created without the component; re-run if needed after compile.");
            }

            var unityPrefabPath = EditorConfig.ToAssetsRelative(prefabPath);
            PrefabUtility.SaveAsPrefabAsset(go, unityPrefabPath, out var success);
            DestroyImmediate(go);
            if (!success)
            {
                throw new Exception($"Failed to save prefab at {unityPrefabPath}");
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
        }

        private static string LoadTemplate(string kind, UIType uiType)
        {
            var path = EditorConfig.GetTemplatePath(kind, uiType);
            return File.Exists(path) ? File.ReadAllText(path, Encoding.UTF8) : null;
        }

        private static string FillTemplate(string template, params (string key, string value)[] pairs)
        {
            var result = template;
            foreach (var (key, value) in pairs)
            {
                result = result.Replace("{{" + key + "}}", value);
            }
            return result;
        }

        private static int StableId(string name)
        {
            unchecked
            {
                var h = 2166136261;
                foreach (var t in name)
                {
                    h ^= t;
                    h *= 16777619;
                }
                return (int)(h & 0x3FFFFFFF);
            }
        }

        private static void GenerateUIIds()
        {
            try
            {
                var validMap = CollectValidNames();

                var filePath = EditorConfig.GetScriptPath(UIConfig.UIIds);
                var genDir = Path.GetDirectoryName(filePath);
                if (!string.IsNullOrEmpty(genDir) && !Directory.Exists(genDir))
                {
                    Directory.CreateDirectory(genDir);
                }

                var sb = new StringBuilder();
                sb.AppendLine(EditorConfig.StartGenerate);
                sb.AppendLine(EditorConfig.UsingCore);
                sb.AppendLine();
                sb.AppendLine(EditorConfig.NamespaceGenerated);
                sb.AppendLine("{");

                WriteClass(sb, UIType.System, validMap[UIType.System]);
                sb.AppendLine();
                WriteClass(sb, UIType.Static, validMap[UIType.Static]);
                sb.AppendLine();
                WriteClass(sb, UIType.Stack, validMap[UIType.Stack]);

                sb.AppendLine("}");
                sb.AppendLine(EditorConfig.EndGenerate);

                AssetDatabase.StartAssetEditing();
                try
                {
                    File.WriteAllText(filePath, sb.ToString(), Encoding.UTF8);
                    AssetDatabase.ImportAsset(EditorConfig.ToAssetsRelative(filePath));
                }
                finally
                {
                    AssetDatabase.StopAssetEditing();
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"❌ UpdateUIIds failed: {ex.Message}\n{ex}");
                throw;
            }
        }

        private static void GenerateUIMapping()
        {
            try
            {
                var validMap = CollectValidNames();

                var filePath = EditorConfig.GetScriptPath(UIConfig.UIMapping);
                var genDir = Path.GetDirectoryName(filePath);
                if (!string.IsNullOrEmpty(genDir) && !Directory.Exists(genDir))
                {
                    Directory.CreateDirectory(genDir);
                }

                // Build base UI mapping and collect layers for static UIs
                var layersMap = new Dictionary<string, List<string>>(StringComparer.Ordinal);

                string QualifyUILayers(string layerName)
                {
                    return $"Domivium.Client.Contents.UI.UILayers.{layerName}";
                }

                void CollectLayersForStaticUI(string uiName)
                {
                    try
                    {
                        var type = UIType.Static;
                        var presenterPath = Path.Combine(type.ToScriptPath(), uiName, type.ToPresenter(uiName) + EditorConfig.CSharpExtension);
                        if (!File.Exists(presenterPath)) return;

                        var text = File.ReadAllText(presenterPath);

                        var marker = "protected override HashSet<UILayer> Layer";
                        var idx = text.IndexOf(marker, StringComparison.Ordinal);
                        if (idx < 0) return;

                        var after = text.IndexOf("=>", idx, StringComparison.Ordinal);
                        if (after < 0) return;

                        var semi = text.IndexOf(';', after);
                        if (semi < 0) return;

                        var expr = text.Substring(after + 2, semi - (after + 2));

                        bool contains(string s)
                        {
                            return expr.IndexOf(s, StringComparison.Ordinal) >= 0;
                        }

                        var layers = new List<string>();
                        if (contains("UILayer.SetDefault"))
                        {
                            layers.Add(QualifyUILayers("Default"));
                        }

                        // Extract UILayers.X identifiers
                        foreach (Match m in Regex.Matches(expr, @"UILayers\s*\.\s*(?<name>[A-Za-z_][A-Za-z0-9_]*)"))
                        {
                            var name = m.Groups["name"].Value;
                            if (string.Equals(name, "HideAll", StringComparison.Ordinal))
                            {
                                // Skip HideAll in mapping to avoid unintended behavior
                            }
                            var q = QualifyUILayers(name);
                            if (!layers.Contains(q)) layers.Add(q);
                        }

                        // For SetWithDefault ensure Default included
                        if (contains("UILayer.SetWithDefault") && !layers.Contains(QualifyUILayers("Default")))
                        {
                            layers.Add(QualifyUILayers("Default"));
                        }

                        // Register collected layers
                        foreach (var layer in layers.Distinct().OrderBy(x => x, StringComparer.Ordinal))
                        {
                            if (!layersMap.TryGetValue(layer, out var list))
                            {
                                list = new List<string>();
                                layersMap[layer] = list;
                            }
                            list.Add($"StaticUIId.{uiName}");
                        }
                    }
                    catch (Exception e)
                    {
                        Debug.LogWarning($"Failed to collect layers for Static UI '{uiName}': {e.Message}");
                    }
                }

                var sb = new StringBuilder();
                sb.AppendLine(EditorConfig.StartGenerate);
                sb.AppendLine(EditorConfig.UsingSystem);
                sb.AppendLine(EditorConfig.UsingGeneric);
                sb.AppendLine(EditorConfig.UsingCore);
                sb.AppendLine();
                sb.AppendLine(EditorConfig.NamespaceGenerated);
                sb.AppendLine("{");
                sb.AppendLine($"\t{EditorConfig.MappingStaticClass}");
                sb.AppendLine("\t{");
                sb.AppendLine($"\t\t{EditorConfig.MappingDictionary} ");
                sb.AppendLine("\t\t{");

                foreach (UIType type in Enum.GetValues(typeof(UIType)))
                {
                    if (!validMap.TryGetValue(type, out var names) || names.Count == 0) continue;

                    sb.AppendLine($"\t\t\t// {type}");
                    foreach (var name in names.OrderBy(n => n))
                    {
                        var keyClass = $"{type}UIId";
                        var contentsNs = EditorConfig.GetContentsNamespaceName(type);
                        var presenterType = $"{contentsNs}.{name}{type}UIPresenter";
                        var viewType = $"{contentsNs}.{name}{type}UIView";
                        var line = $"\t\t\t{{ {keyClass}.{name}, (typeof({presenterType}), typeof({viewType})) }}";
                        sb.AppendLine(line + ",");

                        // Collect layer info only for Static UIs
                        if (type == UIType.Static)
                        {
                            CollectLayersForStaticUI(name);
                        }
                    }
                }

                sb.AppendLine("\t\t};");
                sb.AppendLine();

                // Emit UIsByLayer dictionary
                sb.AppendLine("\t\tpublic static readonly Dictionary<UILayer, HashSet<UIId>> UIsByLayer = new () ");
                sb.AppendLine("\t\t{");
                if (layersMap.Count > 0)
                {
                    foreach (var kv in layersMap.OrderBy(kv => kv.Key, StringComparer.Ordinal))
                    {
                        var layerKey = kv.Key; // already qualified
                        var ids = kv.Value.Distinct().OrderBy(x => x, StringComparer.Ordinal).ToArray();
                        var idsJoined = string.Join(", ", ids);
                        sb.AppendLine($"\t\t\t{{ {layerKey}, new HashSet<UIId> {{ {idsJoined} }} }},");
                    }
                }
                sb.AppendLine("\t\t};");

                sb.AppendLine("\t}");
                sb.AppendLine("}");
                sb.AppendLine(EditorConfig.EndGenerate);

                AssetDatabase.StartAssetEditing();
                try
                {
                    File.WriteAllText(filePath, sb.ToString(), Encoding.UTF8);
                    AssetDatabase.ImportAsset(EditorConfig.ToAssetsRelative(filePath));
                }
                finally
                {
                    AssetDatabase.StopAssetEditing();
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"❌ UpdateUIMapping failed: {ex.Message}\n{ex}");
                throw;
            }
        }

        private static Dictionary<UIType, List<string>> CollectValidNames()
        {
            var validMap = new Dictionary<UIType, List<string>>
            {
                { UIType.System, new List<string>() },
                { UIType.Static, new List<string>() },
                { UIType.Stack, new List<string>() }
            };

            foreach (UIType type in Enum.GetValues(typeof(UIType)))
            {
                var scriptsRoot = type.ToScriptPath();
                if (!Directory.Exists(scriptsRoot)) continue;

                foreach (var dir in Directory.GetDirectories(scriptsRoot))
                {
                    var name = Path.GetFileName(dir);
                    if (string.IsNullOrEmpty(name)) continue;

                    var hasMessage = File.Exists(Path.Combine(dir, type.ToMessage(name) + EditorConfig.CSharpExtension));
                    var hasView = File.Exists(Path.Combine(dir, type.ToView(name) + EditorConfig.CSharpExtension));
                    var hasPresenter = File.Exists(Path.Combine(dir, type.ToPresenter(name) + EditorConfig.CSharpExtension));

                    if (!(hasMessage && hasView && hasPresenter)) continue;

                    var prefabFolder = type.ToPrefabPath();
                    var prefabPath = Path.Combine(prefabFolder, name + EditorConfig.PrefabExtension);
                    if (!File.Exists(prefabPath)) continue;

                    validMap[type].Add(name);
                }
            }

            return validMap;
        }

        private static void UpdateUIContainer()
        {
            try
            {
                var container = _uiContainer as UIContainer ?? AssetDatabase.LoadAssetAtPath<UIContainer>(EditorConfig.UIContainer);
                var collected = new List<UIBehaviour>();

                if (Directory.Exists(EditorConfig.PrefabRootPath))
                {
                    var guids = new List<string>();
                    foreach (UIType type in Enum.GetValues(typeof(UIType)))
                    {
                        var folder = EditorConfig.ToAssetsRelative(type.ToPrefabPath());
                        if (!string.IsNullOrEmpty(folder) && AssetDatabase.IsValidFolder(folder))
                        {
                            guids.AddRange(AssetDatabase.FindAssets("t:Prefab", new[] { folder }));
                        }
                    }

                    foreach (var guid in guids)
                    {
                        var path = AssetDatabase.GUIDToAssetPath(guid);
                        if (string.IsNullOrEmpty(path)) continue;

                        var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                        if (prefab == null) continue;

                        var viewOnAsset = prefab.GetComponentInChildren<UIBehaviour>(true);
                        if (viewOnAsset != null)
                        {
                            collected.Add(viewOnAsset);
                        }
                    }
                }
                else
                {
                    Debug.LogWarning($"UI prefabs root not found at: {EditorConfig.PrefabRootPath}");
                }

                container.UI.Clear();
                foreach (var view in collected.Distinct())
                {
                    container.UI.Add(view);
                }

                EditorUtility.SetDirty(container);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
            }
            catch (Exception ex)
            {
                Debug.LogError($"❌ UpdateUIContainer failed: {ex.Message}\n{ex}");
                throw;
            }
        }

        private static void WriteClass(StringBuilder sb, UIType type, List<string> names)
        {
            var className = $"{type}UIId";
            sb.AppendLine($"\tpublic static class {className}");
            sb.AppendLine("\t{");

            if (names is { Count: > 0 })
            {
                foreach (var uiName in names.OrderBy(n => n))
                {
                    var id = StableId(uiName);
                    sb.AppendLine($"\t\tpublic static UIId {uiName} = {id};");
                }
            }

            sb.AppendLine("\t}");
        }

        private static void DisplayDialog(string message, bool isError = false)
        {
            EditorUtility.DisplayDialog(isError ? "Error" : "Success", message, "OK");
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