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
            if (!File.Exists(LocalDataService.SavePath)) return;

            File.Delete(LocalDataService.SavePath);
            EditorUtility.DisplayDialog("local data", "Delete complete!", "OK");
        }
    }
}