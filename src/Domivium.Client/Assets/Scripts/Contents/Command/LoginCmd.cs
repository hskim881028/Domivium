using Cysharp.Threading.Tasks;
using Domivium.Client.Utility;
using Domivium.Shared.Services;

namespace Domivium.Client.Contents.Command
{
    public sealed class LoginCmd : Command
    {
        public LoginCmd()
        {
            this.Log("LoginCmd");
        }

        public override async UniTask<bool> ExecuteAsync()
        {
            var loginService = NetworkService.CreateService<ILoginService>();
            var response = await loginService.Value.Login();
            if (!NetworkService.HandleResponse(response)) return false;

            this.Log($"[StatusCode]: {response.StatusCode}");
            // SceneScopeManager.LoadScope(SceneScopeId.Lobby);
            return true;
        }

        public override UniTask PostExecuteAsync()
        {
            return UniTask.CompletedTask;
        }
    }
}