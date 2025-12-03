using Cysharp.Threading.Tasks;
using Domivium.Client.Contents.DI;
using Domivium.Client.Contents.UI;
using Domivium.Client.Core.Context;
using Domivium.Client.Core.Input;
using Domivium.Client.Core.Message;
using Domivium.Client.Core.UI.Navigation;

namespace Domivium.Client.Contents.Input.Consumer
{
    public abstract class InputConsumer : IInputConsumer
    {
        protected readonly IAppContext AppContext;
        protected readonly IUINavigation UINavigation;

        public abstract InputPriority Priority { get; }

        protected InputConsumer(IAppContext appContext, IUINavigation uiNavigation)
        {
            AppContext = appContext;
            UINavigation = uiNavigation;
        }

        public abstract bool TryHandle(InputMessage message);

        protected async UniTask ApplyUILayer()
        {
            var layer = UILayers.HideAll;
            if (AppContext.Scene.CurrentValue == SceneScopeIds.Login)
            {
                layer = UILayers.Login;
            }
            else if (AppContext.Scene.CurrentValue == SceneScopeIds.Bootstrap)
            {
                layer = UILayers.Bootstrap;
            }
            else if (AppContext.Scene.CurrentValue == SceneScopeIds.Lobby)
            {
                layer = UILayers.Lobby;
            }
            else if (AppContext.Scene.CurrentValue == SceneScopeIds.Stage)
            {
                layer = UILayers.Stage;
            }

            await UINavigation.ApplyUILayer(layer);
        }
    }
}