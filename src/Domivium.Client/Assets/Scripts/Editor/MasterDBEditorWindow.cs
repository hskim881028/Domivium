using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Domivium.Client.Contents.DI.Container;
using MasterMemory;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.Compilation;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Domivium.Client.Editor
{
    public class MasterDBEditorWindow : EditorWindowBase
    {
        private const string ContinueBuildFlag = "Domivium.MasterDB.ContinueBuild";

        private static Object _configContainer;

        static MasterDBEditorWindow()
        {
            AssemblyReloadEvents.afterAssemblyReload += OnAfterAssemblyReload;
        }

        private static void OnAfterAssemblyReload()
        {
            EditorApplication.delayCall += () =>
            {
                if (!SessionState.GetBool(EditorConfig.MasterDBSessionKey, false)) return;

                var container = _configContainer as ConfigContainer ?? AssetDatabase.LoadAssetAtPath<ConfigContainer>(EditorConfig.ToAssetsRelative(EditorConfig.ConfigContainer));
                var so = new SerializedObject(container);
                so.FindProperty("_masterDB").objectReferenceValue = AssetDatabase.LoadAssetAtPath<TextAsset>(EditorConfig.ToAssetsRelative(EditorConfig.MasterDBPath));
                so.ApplyModifiedProperties();

                EditorUtility.SetDirty(container);
                AssetDatabase.SaveAssets();
                EditorUtility.DisplayDialog("MasterDB", "Build complete!", "OK");
            };
        }

        [MenuItem("Domivium/MasterDB Editor")]
        public static void ShowWindow()
        {
            GetWindow<MasterDBEditorWindow>("MasterDB Editor");
            Refresh();
        }

        protected override void OnEnableInternal()
        {
            SessionState.SetBool(EditorConfig.MasterDBSessionKey, true);
            Refresh();
        }

        protected override void OnDisableInternal()
        {
            SessionState.SetBool(EditorConfig.MasterDBSessionKey, false);
        }

        protected override void OnGUIInternal()
        {
            DrawContainerField();
            DrawButtons();
        }

        private static void Refresh()
        {
            _configContainer = AssetDatabase.LoadAssetAtPath<ConfigContainer>(EditorConfig.ConfigContainer);
        }

        private static void DrawContainerField()
        {
            _configContainer = EditorGUILayout.ObjectField("Container", _configContainer, typeof(ConfigContainer), false);
        }

        private void DrawButtons()
        {
            EditorGUILayout.Space(10);
            using (new EditorGUI.DisabledScope(false))
            {
                // if (GUILayout.Button("Generate for json", ButtonStyle))
                // {
                //     try
                //     {
                //         BuildMasterDBFromAllJson();
                //         EditorPrefs.SetBool(ContinueBuildFlag, true); // flag to continue building after recompiling the script
                //         AssetDatabase.Refresh();
                //         CompilationPipeline.RequestScriptCompilation();
                //         EditorUtility.DisplayDialog("MasterDB (JSON)", "Build complete!", "OK");
                //     }
                //     catch (Exception e)
                //     {
                //         Debug.LogException(e);
                //         EditorUtility.DisplayDialog("MasterDB - Build Error(JSON)", e.Message, "OK");
                //     }
                // }

                if (GUILayout.Button("Generate", ButtonStyle))
                {
                    try
                    {
                        GenerateCsvRowClasses();
                        EditorPrefs.SetBool(ContinueBuildFlag, true); // flag to continue building after recompiling the script
                        AssetDatabase.Refresh();
                        CompilationPipeline.RequestScriptCompilation();
                        EditorUtility.DisplayDialog("MasterDB", "From csv to row class regeneration complete.\nContinue DB build after recompiling.", "OK");
                    }
                    catch (Exception e)
                    {
                        Debug.LogException(e);
                        EditorUtility.DisplayDialog("MasterDB - Error", e.Message, "OK");
                    }
                }
            }
        }

        private static void GenerateCsvRowClasses()
        {
            Directory.CreateDirectory(EditorConfig.RowDataRootPath);
            foreach (var path in Directory.EnumerateFiles(EditorConfig.RowDataRootPath, $"*{EditorConfig.CSharpExtension}", SearchOption.TopDirectoryOnly))
            {
                var txt = File.ReadAllText(path);
                if (txt.Contains(EditorConfig.StartGenerate) && txt.Contains(EditorConfig.EndGenerate))
                {
                    File.Delete(path);
                }
            }

            if (!Directory.Exists(EditorConfig.CsvRootPath))
            {
                Directory.CreateDirectory(EditorConfig.CsvRootPath);
            }

            var csvFiles = Directory.EnumerateFiles(EditorConfig.CsvRootPath, $"*{EditorConfig.CSVExtension}", SearchOption.TopDirectoryOnly).ToArray();
            if (csvFiles.Length == 0)
            {
                throw new Exception($"CSV file does not exist in the Folder: {EditorConfig.CsvRootPath}");
            }

            foreach (var csvPath in csvFiles)
            {
                CreateRowClassFromCsv(csvPath);
            }
        }

        private static void CreateRowClassFromCsv(string csvPath)
        {
            // CSV format: row 1=header, row 2=type, row 3~=data
            var allLines = File.ReadAllLines(csvPath, Encoding.UTF8)
                .Where(l => !string.IsNullOrWhiteSpace(l))
                .ToArray();
            if (allLines.Length < 2)
                throw new Exception($"CSV requires at least 2 lines (header/type): {csvPath}");

            var headers = SplitCsvLine(allLines[0]);
            var types = SplitCsvLine(allLines[1]);
            if (headers.Length != types.Length)
                throw new Exception($"Number of header/type columns mismatch: {Path.GetFileName(csvPath)}");

            // Class name: <filename>Row (e.g. Character.csv -> CharacterRow)
            var fileName = Path.GetFileNameWithoutExtension(csvPath);
            var className = MakeSafeIdentifier(fileName) + "Row";
            var tableName = MakeMemoryTableName(fileName);

            var sb = new StringBuilder();
            sb.AppendLine(EditorConfig.StartGenerate);
            sb.AppendLine("using MasterMemory;");
            sb.AppendLine("using MessagePack;");
            sb.AppendLine();
            sb.AppendLine($"namespace {EditorConfig.RowDataNamespace}");
            sb.AppendLine("{");
            sb.AppendLine($"\t[MemoryTable(\"{tableName}\"), MessagePackObject(true)]");
            sb.AppendLine($"\tpublic class {className}");
            sb.AppendLine("\t{");

            for (var i = 0; i < headers.Length; i++)
            {
                var propName = MakeSafeIdentifier(headers[i]);
                var typeToken = types[i].Trim();

                var (csharpType, comment) = MapCsvTypeToCSharp(typeToken);

                var attr = propName.Equals("Id", StringComparison.OrdinalIgnoreCase) ? "[PrimaryKey] " : "";
                var cmt = string.IsNullOrEmpty(comment) ? string.Empty : $" // {comment}";
                var init = csharpType == "string" ? " = string.Empty;" : string.Empty;
                sb.AppendLine($"\t\t{attr}public {csharpType} {propName} {{ get; set; }}{init}{cmt}");
            }

            sb.AppendLine("\t}");
            sb.AppendLine("}");
            sb.AppendLine(EditorConfig.EndGenerate);

            var outPath = Path.Combine(EditorConfig.RowDataRootPath, className + EditorConfig.CSharpExtension);
            Directory.CreateDirectory(Path.GetDirectoryName(outPath)!);
            File.WriteAllText(outPath, sb.ToString(), new UTF8Encoding(false));
        }

        private static (string typeName, string comment) MapCsvTypeToCSharp(string typeToken)
        {
            var a = typeToken.ToLowerInvariant();
            switch (typeToken.ToLowerInvariant())
            {
                case "int": return ("int", "");
                case "long": return ("long", "");
                case "bool": return ("bool", "");
                case "string": return ("string", "");
                case "percent": return ("int", "percent");
                case "double": return ("double", "");
                case "float": return ("float", "");
                default:
                    Debug.LogWarning($"Unknown type token: {typeToken}, treat as string.");
                    return ("string", "auto: unknown -> string");
            }
        }

        private static string MakeMemoryTableName(string fileStem) => fileStem.Trim().Replace(" ", "_").ToLowerInvariant();

        private static string MakeSafeIdentifier(string raw)
        {
            var s = raw.Trim();

            if (s.Length == 0) s = "Field";
            if (char.IsDigit(s[0])) s = "_" + s;

            var sb = new StringBuilder();
            foreach (var ch in s)
            {
                if (char.IsLetterOrDigit(ch) || ch == '_') sb.Append(ch);
                else sb.Append('_');
            }

            // PascalCase-ish
            var parts = sb.ToString().Split(new[] { '_' }, StringSplitOptions.RemoveEmptyEntries);
            var pascal = string.Concat(parts.Select(p => char.ToUpperInvariant(p[0]) + p.Substring(1)));
            return string.IsNullOrEmpty(pascal) ? "Field" : pascal;
        }

        private static string[] SplitCsvLine(string line) => line.Split(',');

        [DidReloadScripts]
        private static void OnScriptsReloaded()
        {
            if (!EditorPrefs.GetBool(ContinueBuildFlag, false)) return;

            try
            {
                EditorPrefs.DeleteKey(ContinueBuildFlag);
                var builder = new DatabaseBuilder();

                var csvData = GetCsvData();
                foreach (var (rowType, list) in csvData)
                {
                    builder.AppendDynamic(rowType, list);
                }

                var jsonData = GetJsonData();
                foreach (var (rowType, list) in jsonData)
                {
                    builder.AppendDynamic(rowType, list);
                }

                var bytes = builder.Build();
                Directory.CreateDirectory(Path.GetDirectoryName(EditorConfig.MasterDBPath)!);
                File.WriteAllBytes(EditorConfig.MasterDBPath, bytes);

                AssetDatabase.ImportAsset(EditorConfig.ToAssetsRelative(EditorConfig.MasterDBPath));
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                EditorUtility.DisplayDialog("MasterDB - Build Error", e.Message, "OK");
            }
        }

        private static Dictionary<Type, List<object>> GetCsvData()
        {
            var rowTypes = TypeCache.GetTypesWithAttribute<MemoryTableAttribute>()
                .Where(t => t.IsClass && !t.IsAbstract && t.Namespace == EditorConfig.RowDataNamespace)
                .ToArray();

            if (rowTypes.Length == 0)
            {
                Debug.LogWarning($"No row types found. Check namespace({EditorConfig.RowDataNamespace}) and attributes.");
            }

            var result = new Dictionary<Type, List<object>>();
            var builder = new DatabaseBuilder();
            foreach (var rowType in rowTypes)
            {
                var className = rowType.Name; // e.g., CharacterRow
                var stem = className.EndsWith("Row", StringComparison.Ordinal) ? className[..^3] : className;
                var csvPath = Path.Combine(EditorConfig.CsvRootPath, stem + EditorConfig.CSVExtension);

                if (!File.Exists(csvPath))
                {
                    Debug.LogWarning($"No csv: {csvPath} (Type {className}) - Skip");
                    continue;
                }

                var typedList = ParseCsvToTypedList(csvPath, rowType);
                var tableData = ((IEnumerable)typedList).Cast<object>().ToList();
                result.Add(rowType, tableData);
                builder.AppendDynamic(rowType, tableData);
            }

            return result;
        }

        private static object ParseCsvToTypedList(string csvPath, Type rowType)
        {
            var allLines = File.ReadAllLines(csvPath, Encoding.UTF8)
                .Where(l => !string.IsNullOrWhiteSpace(l))
                .ToArray();
            if (allLines.Length < 3)
            {
                throw new Exception($"Insufficient csv data (at least 3 lines): {csvPath}");
            }

            var headers = SplitCsvLine(allLines[0]);
            var types = SplitCsvLine(allLines[1]);
            if (headers.Length != types.Length)
            {
                throw new Exception($"Number of header/type columns mismatch: {Path.GetFileName(csvPath)}");
            }

            var listType = typeof(List<>).MakeGenericType(rowType);
            var list = Activator.CreateInstance(listType);
            var props = headers.Select(h => rowType.GetProperty(MakeSafeIdentifier(h),
                BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase)).ToArray();

            for (var line = 2; line < allLines.Length; line++)
            {
                var row = Activator.CreateInstance(rowType);
                var cols = SplitCsvLine(allLines[line]);

                if (cols.Length != headers.Length)
                {
                    throw new Exception($"{Path.GetFileName(csvPath)} {line + 1} row: column count mismatch");
                }

                for (var i = 0; i < headers.Length; i++)
                {
                    var p = props[i];
                    if (p == null) continue;

                    var token = types[i].Trim().ToLowerInvariant();
                    var valStr = cols[i];

                    object value = token switch
                    {
                        "int" or "percent" => int.Parse(valStr),
                        "long" => long.Parse(valStr, NumberStyles.Integer, CultureInfo.InvariantCulture),
                        "float" => float.Parse(valStr, NumberStyles.Float | NumberStyles.AllowThousands, CultureInfo.InvariantCulture),
                        "double" => double.Parse(valStr, NumberStyles.Float | NumberStyles.AllowThousands, CultureInfo.InvariantCulture),
                        "bool" => ParseBool(valStr),
                        "string" => valStr,
                        _ => valStr // unknown -> string
                    };

                    p.SetValue(row, value);
                }

                listType.GetMethod("Add")!.Invoke(list, new[] { row });
            }

            return list;
        }

        private static bool ParseBool(string s)
        {
            if (bool.TryParse(s, out var b)) return b;

            s = s.Trim().ToLowerInvariant();
            return s switch
            {
                "1" or "yes" or "y" or "true" => true,
                "0" or "no" or "n" or "false" => false,
                _ => throw new Exception($"bool Parsing failed: {s}")
            };
        }

        private static string Stem(Type type)
        {
            var name = type.Name;
            if (name.EndsWith("Row", StringComparison.Ordinal))
            {
                name = name[..^3];
            }

            return name.ToLowerInvariant();
        }

        private static Dictionary<Type, List<object>> GetJsonData()
        {
            var rowTypes = TypeCache.GetTypesWithAttribute<MemoryTableAttribute>()
                .Where(t => t.IsClass && !t.IsAbstract && t.Namespace == EditorConfig.RowDataNamespace)
                .ToArray();

            if (rowTypes.Length == 0)
            {
                Debug.LogWarning($"No row types found. Check namespace({EditorConfig.RowDataNamespace}) and attributes.");
            }

            var rowTypeByStem = rowTypes.ToDictionary(Stem, t => t);

            Directory.CreateDirectory(EditorConfig.JsonRootPath);
            var jsonPaths = Directory.EnumerateFiles(EditorConfig.JsonRootPath, $"*{EditorConfig.JsonExtension}", SearchOption.TopDirectoryOnly).ToArray();
            if (jsonPaths.Length == 0)
            {
                Debug.LogWarning($"No json files in: {EditorConfig.JsonRootPath}");
            }

            var result = new Dictionary<Type, List<object>>();
            foreach (var jsonPath in jsonPaths)
            {
                var text = File.ReadAllText(jsonPath, Encoding.UTF8);
                JsonTablesRoot root;
                try
                {
                    root = JsonConvert.DeserializeObject<JsonTablesRoot>(text);
                }
                catch (Exception ex)
                {
                    Debug.LogWarning($"Skip invalid json: {jsonPath}\n{ex.Message}");
                    continue;
                }

                if (root?.Tables == null) continue;

                foreach (var (key, jsonTable) in root.Tables)
                {
                    var tableKey = (key ?? "").Trim().ToLowerInvariant();
                    if (!rowTypeByStem.TryGetValue(tableKey, out var rowType))
                    {
                        Debug.LogWarning($"No matching Row type for table '{key}' (json: {jsonPath})");
                        continue;
                    }

                    var listObj = ParseJsonTableToTypedList(jsonTable, rowType);
                    if (listObj is not IEnumerable list) continue;

                    if (!result.TryGetValue(rowType, out var bag))
                    {
                        bag = new List<object>();
                        result[rowType] = bag;
                    }
                    bag.AddRange(list.Cast<object>());
                }
            }
            return result;
        }

        private static object ParseJsonTableToTypedList(JsonTable table, Type rowType)
        {
            if (table == null)
            {
                throw new Exception("JsonTable is null");
            }

            if (table.Columns == null || table.Types == null || table.Rows == null)
            {
                throw new Exception("JsonTable must contain columns/types/rows");
            }

            var headers = table.Columns.Select(h => h?.Trim()).ToArray();
            var types = table.Types.Select(t => t?.Trim().ToLowerInvariant()).ToArray();

            if (headers.Length != types.Length)
            {
                throw new Exception($"columns/types length mismatch: {headers.Length} != {types.Length}");
            }

            var props = headers.Select(h => rowType.GetProperty(h,
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.IgnoreCase)).ToArray();

            var list = (IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(rowType));

            foreach (var rowToken in table.Rows) // each rowToken is JArray
            {
                if (rowToken is not JArray rowVals) continue;

                if (rowVals.Count != headers.Length)
                {
                    throw new Exception($"row length mismatch: expected {headers.Length}, got {rowVals.Count}");
                }

                var row = Activator.CreateInstance(rowType);
                for (var i = 0; i < headers.Length; i++)
                {
                    var p = props[i];
                    if (p == null) continue;

                    var jt = rowVals[i];
                    var valStr = jt.Type == JTokenType.String
                        ? jt.Value<string>() ?? string.Empty
                        : jt.ToString(Formatting.None);

                    object value = types[i] switch
                    {
                        "int" or "percent" => int.Parse(valStr),
                        "long" => long.Parse(valStr, NumberStyles.Integer, CultureInfo.InvariantCulture),
                        "float" => float.Parse(valStr, NumberStyles.Float | NumberStyles.AllowThousands, CultureInfo.InvariantCulture),
                        "double" => double.Parse(valStr, NumberStyles.Float | NumberStyles.AllowThousands, CultureInfo.InvariantCulture),
                        "bool" => ParseBool(valStr),
                        "string" => valStr,
                        _ => valStr // unknown -> string
                    };

                    p.SetValue(row, value);
                }

                list.Add(row);
            }

            return list;
        }

        [Serializable]
        private sealed class JsonTablesRoot
        {
            [JsonProperty("tables")] public Dictionary<string, JsonTable> Tables { get; set; }
        }

        [Serializable]
        private sealed class JsonTable
        {
            [JsonProperty("columns")] public string[] Columns { get; set; }
            [JsonProperty("types")] public string[] Types { get; set; }
            [JsonProperty("rows")] public JArray Rows { get; set; }
        }
    }
}