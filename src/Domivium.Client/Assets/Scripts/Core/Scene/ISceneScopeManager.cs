using System;

namespace Domivium.Client.Core.Scene
{
    public interface ISceneScopeManager : IDisposable
    {
        public void LoadScope(SceneScopeId sceneScopeId);
    }
}