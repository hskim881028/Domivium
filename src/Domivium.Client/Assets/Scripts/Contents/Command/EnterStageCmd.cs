using Cysharp.Threading.Tasks;
using Domivium.Client.Core.Scene;
using Domivium.Client.Core.Utility;

namespace Domivium.Client.Contents.Command
{
    public sealed class EnterStageCmd : Command
    {
        public EnterStageCmd()
        {
            this.Log();
        }

        public override UniTask<bool> ExecuteAsync()
        {
            SceneService.LoadScope(SceneScopeId.Stage);
            return UniTask.FromResult(true);
        }

        public override UniTask PostExecuteAsync()
        {
            return UniTask.CompletedTask;
        }
    }
}