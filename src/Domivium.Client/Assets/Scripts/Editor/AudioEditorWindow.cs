using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Domivium.Client.Contents.DI.Container;
using Domivium.Client.Core.Audio;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Domivium.Client.Editor
{
    public class AudioEditorWindow : EditorWindowBase
    {
        private static Object _audioContainer;

        [MenuItem("Domivium/Audio Editor")]
        public static void ShowWindow()
        {
            Refresh();
            EditorSceneManager.OpenScene(EditorConfig.WorkspaceScene);
            GetWindow<AudioEditorWindow>("Audio Editor");
        }

        protected override void OnEnableInternal()
        {
            Refresh();
        }

        protected override void OnDisableInternal() { }

        protected override void OnGUIInternal()
        {
            DrawContainerField();
            DrawButton();
        }

        private static void Refresh()
        {
            _audioContainer = AssetDatabase.LoadAssetAtPath<AudioContainer>(EditorConfig.AudioContainer);
        }

        private static void DrawContainerField()
        {
            _audioContainer = EditorGUILayout.ObjectField("Container", _audioContainer, typeof(AudioContainer), false);
        }

        private void DrawButton()
        {
            EditorGUILayout.Space(10);
            using (new EditorGUI.DisabledScope(false))
            {
                if (GUILayout.Button("Generate", ButtonStyle))
                {
                    Generate();
                }
            }
        }

        private void Generate()
        {
            GenerateIds();
            GenerateMapping();
        }

        private void GenerateIds()
        {
            try
            {
                var filePath = EditorConfig.GetGeneratedScriptPath(EditorConfig.Audio, EditorConfig.AudioIds);
                var genDir = Path.GetDirectoryName(filePath);
                if (!string.IsNullOrEmpty(genDir) && !Directory.Exists(genDir))
                {
                    Directory.CreateDirectory(genDir);
                }

                var sb = new StringBuilder();
                sb.AppendLine(EditorConfig.StartGenerate);
                sb.AppendLine(EditorConfig.UsingAudioCore);
                sb.AppendLine();
                sb.AppendLine(EditorConfig.AudioNamespaceGenerated);
                sb.AppendLine("{");

                var resources = CollectResources();
                WriteClass(sb, AudioParam.BGM, resources[AudioParam.BGM]);
                sb.AppendLine();
                WriteClass(sb, AudioParam.SFX, resources[AudioParam.SFX]);
                sb.AppendLine();
                WriteClass(sb, AudioParam.UI, resources[AudioParam.UI]);

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
                Debug.LogError($"❌ Update AudioIds failed: {ex.Message}\n{ex}");
                throw;
            }
        }

        private void GenerateMapping()
        {
            try
            {
                var filePath = EditorConfig.GetGeneratedScriptPath(EditorConfig.Audio, EditorConfig.AudioMapping);
                var genDir = Path.GetDirectoryName(filePath);
                if (!string.IsNullOrEmpty(genDir) && !Directory.Exists(genDir))
                {
                    Directory.CreateDirectory(genDir);
                }
                
                var sb = new StringBuilder();
                sb.AppendLine(EditorConfig.StartGenerate);
                sb.AppendLine(EditorConfig.UsingGeneric);
                sb.AppendLine(EditorConfig.UsingAudioCore);
                sb.AppendLine();
                sb.AppendLine(EditorConfig.AudioNamespaceGenerated);
                sb.AppendLine("{");
                sb.AppendLine($"\t{EditorConfig.AudioMappingClass}");
                sb.AppendLine("\t{");
                sb.AppendLine($"\t\t{EditorConfig.AudioMappingDictionary} ");
                sb.AppendLine("\t\t{");
                
                var resources = CollectResources();
                foreach (var (param, dic) in resources)
                {
                    var className = $"{param.AsPrimitive()}{nameof(AudioId)}";
                    sb.AppendLine($"\t\t\t// {param.AsPrimitive()}");
                    foreach (var (key, value) in dic)
                    {
                        sb.AppendLine($"\t\t\t{{ {className}.{key}, \"{value}\" }},");
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
                Debug.LogError($"❌ Update AudioMapping failed: {ex.Message}\n{ex}");
                throw;
            }
        }

        private static Dictionary<AudioParam, Dictionary<string, string>> CollectResources()
        {
            var validMap = new Dictionary<AudioParam, Dictionary<string, string>>
            {
                { AudioParam.BGM, new Dictionary<string, string>() },
                { AudioParam.SFX, new Dictionary<string, string>() },
                { AudioParam.UI, new Dictionary<string, string>() }
            };

            foreach (var (param, dic) in validMap)
            {
                var resourcePath = param.ToAudioResourcePath();
                if (!Directory.Exists(resourcePath)) continue;

                foreach (var file in Directory.GetFiles(resourcePath))
                {
                    if (Path.GetExtension(file).Equals(".meta", StringComparison.OrdinalIgnoreCase)) continue;
                    
                    var name = Path.GetFileNameWithoutExtension(file);
                    if (string.IsNullOrEmpty(name)) continue;

                    var key = string.Join("",
                        name.Split('_', StringSplitOptions.RemoveEmptyEntries)
                            .Select(word => char.ToUpperInvariant(word[0]) + word[1..]));
                    dic[key] = name;
                }
            }

            return validMap;
        }

        private static void WriteClass(StringBuilder sb, AudioParam param, Dictionary<string, string> dictionary)
        {
            var className = $"{param.AsPrimitive()}{nameof(AudioId)}";
            sb.AppendLine($"\tpublic static class {className}");
            sb.AppendLine("\t{");

            if (dictionary is { Count: > 0 })
            {
                foreach (var (key, name) in dictionary.OrderBy(n => n.Key))
                {
                    var id = EditorUtils.StableId(key);
                    sb.AppendLine($"\t\tpublic static {nameof(AudioId)} {key} = new({id});");
                }
            }

            sb.AppendLine("\t}");
        }
    }
}