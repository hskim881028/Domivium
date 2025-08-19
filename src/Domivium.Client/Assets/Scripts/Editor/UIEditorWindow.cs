using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Domivium.Client.Core.UI;
using Domivium.Client.Core.UI.View;
using Domivium.Client.Core.Utility;
using Domivium.Client.DI;
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
            Refresh();
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
            var selectedUIType = EditorPrefs.GetString(EditorConfig.SelectedTypeKey, string.Empty);
            if (!Enum.TryParse<UIType>(selectedUIType, out var uiType)) return;

            var uiName = EditorPrefs.GetString(EditorConfig.SelectedNameKey, string.Empty);
            if (string.IsNullOrEmpty(uiName)) return;

            CreatePrefabWithView(uiType, uiName);

            EditorApplication.delayCall += () =>
            {
                RefreshSettings();
                EditorPrefs.DeleteKey(EditorConfig.SelectedTypeKey);
                EditorPrefs.DeleteKey(EditorConfig.SelectedNameKey);
                EditorUtility.DisplayDialog("Success", $"[Type] {uiType}\n[Name] {uiName}", "OK");
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
                _buttonStyle = new GUIStyle(GUI.skin.button)
                {
                    fontSize = 12,
                    padding = new RectOffset(15, 15, 5, 5),
                    margin = new RectOffset(5, 5, 5, 5),
                    normal =
                    {
                        background = CreateColorTexture(new Color(0.23f, 0.35f, 0.48f)),
                        textColor = new Color(0.9f, 0.9f, 0.9f)
                    },
                    hover =
                    {
                        background = CreateColorTexture(new Color(0.28f, 0.45f, 0.65f)),
                        textColor = Color.white
                    },
                    active =
                    {
                        background = CreateColorTexture(new Color(0.2f, 0.3f, 0.4f)),
                        textColor = Color.white
                    }
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
            var uiTypes = Enum.GetValues(typeof(UIType)).Cast<UIType>().ToArray();
            var newUIType = (UIType)EditorGUILayout.IntPopup("Type",
                (int)_uiType,
                uiTypes.Select(t => t.ToString()).ToArray(),
                uiTypes.Select(t => (int)t).ToArray());

            if (_uiType != newUIType)
            {
                _uiType = newUIType;
            }
        }

        private void DrawUINameField()
        {
            var newUIName = EditorGUILayout.TextField("Name", _uiName);
            if (newUIName != _uiName)
            {
                _uiName = CapitalizeFirstLetter(newUIName);
            }
        }

        private void DrawButtons()
        {
            EditorGUILayout.Space(10);
            if (GUILayout.Button("Refresh", _buttonStyle))
            {
                _uiName = string.Empty;
                RefreshSettings();
            }

            if (GUILayout.Button("Generate", _buttonStyle))
            {
                GenerateUI();
                _uiName = string.Empty;
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
                EditorUtility.DisplayDialog("Generate UI", "Name is empty or invalid.", "OK");
                return;
            }

            try
            {
                // 0) Duplicate checks (scripts folder, prefab, UIIds entry)
                if (HasDuplicates(_uiType, _uiName)) return;

                EditorPrefs.SetString(EditorConfig.SelectedNameKey, _uiName);
                EditorPrefs.SetString(EditorConfig.SelectedTypeKey, _uiType.ToString());

                // 1) Create scripts folder and files
                var scriptsFolder = CreateScriptsFolder(_uiType, _uiName);
                CreateMessageScript(_uiType, _uiName, scriptsFolder);
                CreateViewScript(_uiType, _uiName, scriptsFolder);
                CreatePresenterScript(_uiType, _uiName, scriptsFolder);

                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
            catch (Exception ex)
            {
                Debug.LogError($"❌ UI generation failed: {ex.Message}\n{ex}");
            }
        }

        private static bool HasDuplicates(UIType uiType, string uiName)
        {
            // Scripts folder duplicate
            var basePath = uiType.ToScriptPath();
            var folder = Path.Combine(basePath, uiName);
            if (Directory.Exists(folder))
            {
                EditorUtility.DisplayDialog("Generate UI", $"Scripts folder already exists: {folder}", "OK");
                return true;
            }

            // Prefab duplicate
            var prefabPath = uiType.ToPrefabPath();
            var fullPrefab = Path.Combine(prefabPath, uiName + EditorConfig.PrefabExtension);
            if (File.Exists(fullPrefab))
            {
                EditorUtility.DisplayDialog("Generate UI", $"Prefab already exists: {fullPrefab}", "OK");
                return true;
            }

            // UIIds duplicate
            var idsFile = EditorConfig.GetScriptPath(UIConfig.UIIds);
            if (File.Exists(idsFile))
            {
                var idsContent = File.ReadAllText(idsFile);
                var pattern = $@"class\\s+{uiType}{EditorConfig.UIId}[\n\r\s]*{{[\n\r\s\S]*?public\\s+static\\s+UIId\\s+{uiName}\\s*=";
                if (Regex.IsMatch(idsContent, pattern))
                {
                    EditorUtility.DisplayDialog("Generate UI", $"UIId for '{uiName}' already exists in UIIds.cs.", "OK");
                    return true;
                }
            }

            return false;
        }

        private static string CreateScriptsFolder(UIType uiType, string uiName)
        {
            var basePath = uiType.ToScriptPath();
            if (!Directory.Exists(basePath)) Directory.CreateDirectory(basePath);
            var folder = Path.Combine(basePath, uiName);
            Directory.CreateDirectory(folder);
            AssetDatabase.ImportAsset(EditorConfig.ToAssetsRelative(folder));
            return folder;
        }

        private static void CreateMessageScript(UIType uiType, string uiName, string folder)
        {
            var ns = EditorConfig.GetContentsNamespaceName(uiType);
            var interfaceName = uiType.ToMessage(uiName);
            var template = LoadTemplate("Message", uiType);
            if (template == null)
            {
                EditorUtility.DisplayDialog("Generate UI", $"Could not find Message template. Path: {EditorConfig.GetTemplatePath("Message", uiType)}", "OK");
                return;
            }
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

        private static void CreateViewScript(UIType uiType, string uiName, string folder)
        {
            var ns = EditorConfig.GetContentsNamespaceName(uiType);
            var viewName = uiType.ToView(uiName);
            var messageName = uiType.ToMessage(uiName);
            var template = LoadTemplate("View", uiType);
            if (template == null)
            {
                EditorUtility.DisplayDialog("Generate UI", $"Could not find View template. Path: {EditorConfig.GetTemplatePath("View", uiType)}", "OK");
                return;
            }
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

        private static void CreatePresenterScript(UIType uiType, string uiName, string folder)
        {
            var ns = EditorConfig.GetContentsNamespaceName(uiType);
            var presenterName = uiType.ToPresenter(uiName);
            var viewName = uiType.ToView(uiName);
            var messageName = uiType.ToMessage(uiName);
            var template = LoadTemplate("Presenter", uiType);
            if (template == null)
            {
                EditorUtility.DisplayDialog("Generate UI", $"Could not find Presenter template. Path: {EditorConfig.GetTemplatePath("Presenter", uiType)}", "OK");
                return;
            }
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
            rect.SetParent(FindFirstObjectByType<Canvas>().transform);
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
            AssetDatabase.Refresh();
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

        private static void GenerateUIIds()
        {
            try
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

                WriteClass(sb, UIType.System, validMap[UIType.System], 0);
                sb.AppendLine();
                WriteClass(sb, UIType.Static, validMap[UIType.Static], 100);
                sb.AppendLine();
                WriteClass(sb, UIType.Stack, validMap[UIType.Stack], 1000);

                sb.AppendLine("}");
                sb.AppendLine(EditorConfig.EndGenerate);

                File.WriteAllText(filePath, sb.ToString(), Encoding.UTF8);
                AssetDatabase.ImportAsset(EditorConfig.ToAssetsRelative(filePath));
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
                // Collect valid UI names per type (same criteria as GenerateUIIds)
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

                        // Prefab existence check in type-specific prefab folder
                        var prefabFolder = type.ToPrefabPath();
                        var prefabPath = Path.Combine(prefabFolder, name + EditorConfig.PrefabExtension);
                        if (!File.Exists(prefabPath)) continue;

                        validMap[type].Add(name);
                    }
                }

                // Write UIMapping.cs from scratch
                var filePath = EditorConfig.GetScriptPath(UIConfig.UIMapping);
                var genDir = Path.GetDirectoryName(filePath);
                if (!string.IsNullOrEmpty(genDir) && !Directory.Exists(genDir))
                {
                    Directory.CreateDirectory(genDir);
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

                    sb.AppendLine($"\t\t\t// {type.ToString()}");
                    foreach (var name in names.OrderBy(n => n))
                    {
                        // Key: <Type>UIId.<Name>
                        var keyClass = $"{type}UIId";
                        // Presenter: typeof(<ContentsNamespace>.<Name><Type>UIPresenter)
                        var contentsNs = EditorConfig.GetContentsNamespaceName(type);
                        var presenterType = $"{contentsNs}.{name}{type}UIPresenter";
                        // View type: typeof(<ContentsNamespace>.<Name><Type>UIView)
                        var viewType = $"{contentsNs}.{name}{type}UIView";

                        var line = $"\t\t\t{{ {keyClass}.{name}, (typeof({presenterType}), typeof({viewType})) }}";
                        sb.AppendLine(line + ",");
                    }
                }

                sb.AppendLine("\t\t};");
                sb.AppendLine("\t}");
                sb.AppendLine("}");
                sb.AppendLine(EditorConfig.EndGenerate);

                File.WriteAllText(filePath, sb.ToString(), Encoding.UTF8);
                AssetDatabase.ImportAsset(EditorConfig.ToAssetsRelative(filePath));
            }
            catch (Exception ex)
            {
                Debug.LogError($"❌ UpdateUIMapping failed: {ex.Message}\n{ex}");
                throw;
            }
        }

        private static void UpdateUIContainer()
        {
            try
            {
                var container = _uiContainer as UIContainer;
                if (container == null)
                {
                    container = AssetDatabase.LoadAssetAtPath<UIContainer>(EditorConfig.UIContainer);
                }

                var collected = new List<UIViewBase>();
                if (Directory.Exists(EditorConfig.PrefabRootPath))
                {
                    var guids = new List<string>();
                    foreach (UIType type in Enum.GetValues(typeof(UIType)))
                    {
                        guids.AddRange(AssetDatabase.FindAssets("t:Prefab", new[] { EditorConfig.ToAssetsRelative(type.ToPrefabPath()) }));
                    }

                    foreach (var guid in guids)
                    {
                        var path = AssetDatabase.GUIDToAssetPath(guid);
                        if (string.IsNullOrEmpty(path)) continue;

                        var view = AssetDatabase.LoadAssetAtPath<UIViewBase>(path);
                        if (view == null)
                        {
                            var assets = AssetDatabase.LoadAllAssetsAtPath(path);
                            view = assets?.OfType<UIViewBase>().FirstOrDefault();
                        }

                        if (view != null)
                        {
                            collected.Add(view);
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
                AssetDatabase.Refresh();
            }
            catch (Exception ex)
            {
                Debug.LogError($"❌ UpdateUIContainer failed: {ex.Message}\n{ex}");
                throw;
            }
        }

        private static void WriteClass(StringBuilder sb, UIType type, List<string> names, int startId)
        {
            var className = $"{type}UIId";
            sb.AppendLine($"\tpublic static class {className}");
            sb.AppendLine("\t{");

            if (names is { Count: > 0 })
            {
                foreach (var (uiName, idx) in names.OrderBy(n => n).Select((n, i) => (n, i)))
                {
                    var id = startId + idx;
                    sb.AppendLine($"\t\tpublic static UIId {uiName} = {id};");
                }
            }

            sb.AppendLine("\t}");
        }

        private Texture2D CreateColorTexture(Color color)
        {
            var texture = new Texture2D(1, 1);
            texture.SetPixel(0, 0, color);
            texture.Apply();
            return texture;
        }
    }
}