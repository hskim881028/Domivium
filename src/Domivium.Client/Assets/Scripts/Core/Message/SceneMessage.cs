using Domivium.Client.Core.Scene;

namespace Domivium.Client.Core.Message
{
    public readonly struct SceneMessage
    {
        public SceneMessageType Type { get; }
        public SceneScope SceneScope { get; }

        private SceneMessage(SceneMessageType type, SceneScope sceneScope)
        {
            Type = type;
            SceneScope = sceneScope;
        }

        public static SceneMessage Load(SceneScope sceneScope) => new(SceneMessageType.Load, sceneScope);

        public static SceneMessage Unload(SceneScope sceneScope) => new(SceneMessageType.Unload, sceneScope);
    }
}