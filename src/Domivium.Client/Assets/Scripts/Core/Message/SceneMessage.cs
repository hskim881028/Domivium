using Domivium.Client.Core.Scene;

namespace Domivium.Client.Core.Message
{
    public readonly struct SceneMessage
    {
        public SceneMessageType Type { get; }
        public SceneScope SceneScope { get; }

        private SceneMessage(SceneMessageType type)
        {
            Type = type;
            SceneScope = null;
        }

        private SceneMessage(SceneMessageType type, SceneScope sceneScope)
        {
            Type = type;
            SceneScope = sceneScope;
        }

        public static SceneMessage Load(SceneScope sceneScope)
        {
            return new SceneMessage(SceneMessageType.Load, sceneScope);
        }

        public static SceneMessage Unload()
        {
            return new SceneMessage(SceneMessageType.Unload);
        }
    }
}