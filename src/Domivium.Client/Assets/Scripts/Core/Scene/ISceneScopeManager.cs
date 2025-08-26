using System;

namespace Domivium.Client.Core.Scene
{
    public interface ISceneScopeManager : IDisposable
    {
        public void LoadScope<T>(SceneScopeId sceneScopeId) where T : SceneScope;
    }
}