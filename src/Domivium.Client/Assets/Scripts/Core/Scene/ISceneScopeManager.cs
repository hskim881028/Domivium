namespace Domivium.Client.Core.Scene
{
    public interface ISceneScopeManager
    {
        public void LoadScope<T>(SceneScopeId sceneScopeId) where T : SceneScope;
    }
}