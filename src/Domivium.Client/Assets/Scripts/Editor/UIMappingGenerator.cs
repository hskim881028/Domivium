using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Domivium.Client.Core.UI;
using UnityEditor;
using UnityEngine;

namespace Domivium.Client.Editor
{
    public static class UIMappingGenerator
    {
        private class UIData
        {
            public Dictionary<string, int> IdMap { get; } = new();
            public List<string> Entries { get; } = new();
        }

        private record UIFileInfo(string EnumField, int Id, string MappingEntry);

        private record UITypeInfo(EditorConfig.UIType Type, int StartId);

        private static readonly List<UITypeInfo> UITypes = new()
        {
            new UITypeInfo(EditorConfig.UIType.Static, UIOrder.StaticId),
            new UITypeInfo(EditorConfig.UIType.Stack, UIOrder.StackId),
            new UITypeInfo(EditorConfig.UIType.System, UIOrder.SystemId)
        };

        [MenuItem("Domivium/Generate UI Mapping")]
        public static void GenerateUI()
        {
            try
            {
                var uiDataMap = CollectUIData();
                GenerateFiles(uiDataMap);

                AssetDatabase.Refresh();
                Debug.Log($"✅ Generated <color=#81C784>{UIConfig.UIIds}.cs</color> and <color=#81C784>{UIConfig.UIMapping}.cs</color> successfully.");
            }
            catch (Exception ex)
            {
                Debug.LogError($"❌ Failed to generate UI mapping: {ex.Message}");
                throw;
            }
        }

        private static Dictionary<EditorConfig.UIType, UIData> CollectUIData()
        {
            var uiDataMap = new Dictionary<EditorConfig.UIType, UIData>();

            foreach (var uiType in UITypes)
            {
                uiDataMap[uiType.Type] = new UIData();
            }

            foreach (var uiType in UITypes)
            {
                ProcessUIType(uiType, uiDataMap[uiType.Type]);
            }

            return uiDataMap;
        }

        private static void ProcessUIType(UITypeInfo uiType, UIData uiData)
        {
            var category = uiType.Type.ToString();
            var uiPath = $"{EditorConfig.UI}/{category}";
            var fullPath = $"{EditorConfig.ResourcesPath}/{uiPath}";

            if (!Directory.Exists(fullPath))
            {
                throw new DirectoryNotFoundException($"UI directory not found: {fullPath}");
            }

            var prefabFiles = Directory.GetFiles(fullPath, EditorConfig.PrefabSearchPattern, SearchOption.AllDirectories);
            var currentId = uiType.StartId;

            foreach (var file in prefabFiles)
            {
                var fileName = Path.GetFileNameWithoutExtension(file);
                var uiSuffix = $"{category}{EditorConfig.UI}";
                if (!IsValidUIFile(fileName, uiSuffix)) continue;

                var uiInfo = CreateUIInfo(file, fileName, uiType.Type, currentId++);
                uiData.IdMap[uiInfo.EnumField] = uiInfo.Id;
                uiData.Entries.Add(uiInfo.MappingEntry);
            }
        }

        private static bool IsValidUIFile(string fileName, string expectedSuffix)
        {
            return fileName.EndsWith(EditorConfig.UI) && fileName.EndsWith(expectedSuffix);
        }

        private static UIFileInfo CreateUIInfo(string file, string fileName, EditorConfig.UIType uiType, int id)
        {
            var category = uiType.ToString();
            var uiSuffix = $"{category}{EditorConfig.UI}";
            var enumName = $"{uiSuffix}{EditorConfig.Id}";

            var enumField = fileName.Replace(uiSuffix, string.Empty);
            var typeName = $"{fileName}{EditorConfig.Presenter}";
            var relativePath = file
                .Replace($"{EditorConfig.ResourcesPath}/", string.Empty)
                .Replace(EditorConfig.PrefabExtension, string.Empty)
                .Replace("\\", "/");

            var entry = $"{{ {enumName}.{enumField}, (typeof({typeName}), \"{relativePath}\") }}";

            return new UIFileInfo(enumField, id, entry);
        }

        private static void GenerateFiles(Dictionary<EditorConfig.UIType, UIData> uiDataMap)
        {
            if (!Directory.Exists(EditorConfig.GeneratedPath))
            {
                Directory.CreateDirectory(EditorConfig.GeneratedPath);
            }

            var uiIdsContent = GenerateUIIds(uiDataMap);
            var uiMappingContent = GenerateUIMapping(uiDataMap);

            File.WriteAllText($"{EditorConfig.GeneratedPath}/{UIConfig.UIIds}{EditorConfig.CSharpExtension}", uiIdsContent);
            File.WriteAllText($"{EditorConfig.GeneratedPath}/{UIConfig.UIMapping}{EditorConfig.CSharpExtension}", uiMappingContent);
        }

        private static string GenerateUIIds(Dictionary<EditorConfig.UIType, UIData> uiDataMap)
        {
            var template = new StringBuilder();
            template.AppendLine(EditorConfig.StartGenerate);
            template.AppendLine(EditorConfig.UsingCore);
            template.AppendLine();
            template.AppendLine(EditorConfig.NamespaceGenerated);
            template.AppendLine("{");

            var isFirst = true;
            foreach (var uiType in UITypes)
            {
                if (!isFirst) template.AppendLine();
                template.AppendLine(GenerateIdClass(uiType, uiDataMap[uiType.Type].IdMap));
                isFirst = false;
            }

            template.AppendLine("}");
            template.Append(EditorConfig.EndGenerate);
            return template.ToString();
        }

        private static string GenerateIdClass(UITypeInfo uiType, Dictionary<string, int> idMap)
        {
            var sb = new StringBuilder();
            sb.AppendLine($"\t{EditorConfig.ToClass(uiType.Type)}");
            sb.AppendLine("\t{");

            if (idMap.Count > 0)
            {
                foreach (var kvp in idMap.OrderBy(x => x.Value))
                {
                    sb.AppendLine($"\t\t{EditorConfig.ToUIId(kvp)}");
                }
            }

            sb.Append("\t}");
            return sb.ToString();
        }

        private static string GenerateUIMapping(Dictionary<EditorConfig.UIType, UIData> uiDataMap)
        {
            var template = new StringBuilder();
            template.AppendLine(EditorConfig.StartGenerate);

            // Using statements
            template.AppendLine(EditorConfig.UsingSystem);
            template.AppendLine(EditorConfig.UsingGeneric);
            template.AppendLine(EditorConfig.UsingCore);

            foreach (var uiType in UITypes.Where(t => uiDataMap[t.Type].Entries.Count > 0))
            {
                template.AppendLine(EditorConfig.ToUsing(uiType.Type));
            }

            template.AppendLine();
            template.AppendLine(EditorConfig.NamespaceGenerated);
            template.AppendLine("{");
            template.AppendLine($"\t{EditorConfig.MappingStaticClass}");
            template.AppendLine("\t{");
            template.AppendLine($"\t\t{EditorConfig.MappingDictionary}");
            template.AppendLine("\t\t{");

            foreach (var uiType in UITypes)
            {
                var entries = uiDataMap[uiType.Type].Entries;
                if (entries.Count > 0)
                {
                    var category = uiType.Type.ToString();
                    template.AppendLine($"\t\t\t// {category}");
                    foreach (var entry in entries.OrderBy(x => x))
                    {
                        template.AppendLine($"\t\t\t{entry},");
                    }
                }
            }

            template.AppendLine("\t\t};");
            template.AppendLine("\t}");
            template.AppendLine("}");
            template.Append(EditorConfig.EndGenerate);
            return template.ToString();
        }
    }
}