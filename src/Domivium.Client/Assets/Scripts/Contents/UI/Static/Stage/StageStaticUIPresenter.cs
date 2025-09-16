using System.Collections.Generic;
using Domivium.Client.Contents.Audio.Generated;
using Domivium.Client.Contents.Commands;
using Domivium.Client.Contents.DI;
using Domivium.Client.Contents.ReadModels;
using Domivium.Client.Contents.Services;
using Domivium.Client.Core.Audio;
using Domivium.Client.Core.UI;
using Domivium.Client.Core.UI.Navigation;
using Domivium.Client.Core.UI.Presenter;

namespace Domivium.Client.Contents.UI.Static
{
    public class StageStaticUIPresenter : StaticUIPresenter<StageStaticUIView, IStageStaticUIMessage>, IStageStaticUIMessage
    {
        private readonly SceneService _sceneService;
        private readonly ITowerPlacementCommand _towerPlacementCommand;
        private readonly IPointerReadModel _pointerRead;
        private readonly ICameraReadModel _cameraReadModel;

        protected override HashSet<UILayer> Layer => UILayer.Set(UILayers.Stage);

        public override UIPriority Priority => UIPriorities.Stage;

        public StageStaticUIPresenter(
            StageStaticUIView view,
            IUINavigation navigation,
            IAudioController audioController,
            SceneService sceneService,
            ITowerPlacementCommand towerPlacementCommand,
            IPointerReadModel pointerRead)
            : base(view, navigation, audioController)
        {
            _sceneService = sceneService;
            _towerPlacementCommand = towerPlacementCommand;
            _pointerRead = pointerRead;
        }

        public void EnterLobby()
        {
            _sceneService.Load(SceneScopeIds.Lobby);
        }

        public void SelectTower(int index)
        {
            AudioController.PlayUI(UIAudioId.Click);
            _towerPlacementCommand.Show(index);
            _towerPlacementCommand.Update(_pointerRead.Current);
        }

        public void Cancel()
        {
            _towerPlacementCommand.Hide();
        }
    }
}