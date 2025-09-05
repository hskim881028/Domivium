using UnityEditor;
using UnityEditor.SceneManagement;

namespace Domivium.Client.Editor
{
    public static class SceneEditor
    {
        [MenuItem("Scenes/Application")]
        public static void ApplicationScene()
        {
            EditorSceneManager.OpenScene(EditorConfig.ApplicationScene);
        }

        [MenuItem("Scenes/Workspace")]
        public static void WorkspaceScene()
        {
            EditorSceneManager.OpenScene(EditorConfig.WorkspaceScene);
        }
    }
}