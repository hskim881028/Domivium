using Cysharp.Threading.Tasks;
using Domivium.Client.Contents.Command;
using Domivium.Client.Contents.Services;
using Domivium.Client.Core.Command;
using Domivium.Client.Core.Scene;
using UnityEngine.InputSystem;
using VContainer.Unity;

namespace Domivium.Client.DI
{
    public sealed class ApplicationEntry : IStartable, ITickable
    {
        private readonly ICommandExecutor _commandExecutor;

        public ApplicationEntry(
            ISceneScopeManager sceneScopeManager,
            ICommandExecutor commandExecutor,
            NetworkService networkService,
            CameraService cameraService,
            SceneService sceneService)
        {
            networkService.Connect();
            _commandExecutor = commandExecutor;
        }

        public void Start()
        {
            _commandExecutor.Enqueue<LoginCmd>();
        }

        public void Tick()
        {
            ForTest();
            _commandExecutor.TickAsync().Forget();
        }

        private void ForTest()
        {
            if (Keyboard.current.spaceKey.wasPressedThisFrame)
            {
                _commandExecutor.Enqueue<EnterStageCmd>();
            }
        }
    }
}