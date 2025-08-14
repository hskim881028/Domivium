using Cysharp.Threading.Tasks;
using Domivium.Client.Core.Scene;
using Domivium.Client.Core.Utility;
using Domivium.Shared.Services;

namespace Domivium.Client.Contents.Command
{
    public sealed class LoginCmd : Command
    {
        public LoginCmd()
        {
            this.Log();
        }

        public override async UniTask<bool> ExecuteAsync()
        {
            var loginService = NetworkService.CreateService<ILoginService>();
            var response = await loginService.Value.Login();
            if (!NetworkService.HandleResponse(response)) return false;

            this.Log($"[StatusCode]: {response.StatusCode}");
            SceneService.LoadScope(SceneScopeId.Lobby);
            return true;
        }

        public override UniTask PostExecuteAsync()
        {
            return UniTask.CompletedTask;
        }
    }
}