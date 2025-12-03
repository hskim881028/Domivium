using System.IO;
using Domivium.Client.Contents.Services;
using UnityEditor;

namespace Domivium.Client.Editor
{
    public static class LocalDataEditor
    {
        [MenuItem("Domivium/Delete local data")]
        public static void ApplicationScene()
        {
            if (!Directory.Exists(LocalDataService.LocalDataDirectory)) return;

            Directory.Delete(LocalDataService.LocalDataDirectory, true);
            EditorUtility.DisplayDialog("local data", "Delete complete!", "OK");
        }
    }
}