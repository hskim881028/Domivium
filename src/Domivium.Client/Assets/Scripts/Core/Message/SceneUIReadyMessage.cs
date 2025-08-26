using Domivium.Client.Core.Scene;

namespace Domivium.Client.Core.Message
{
    public readonly struct SceneUIReadyMessage
    {
        public SceneScopeId SceneScopeId { get; }

        private SceneUIReadyMessage(SceneScopeId sceneScopeId)
        {
            SceneScopeId = sceneScopeId;
        }

        public static SceneUIReadyMessage Ready(SceneScopeId sceneScopeId) => new(sceneScopeId);
    }
}