using System.Collections.Generic;
using Domivium.Client.Contents.DI;
using Domivium.Client.Contents.Services;
using Domivium.Client.Contents.System.Model;
using Domivium.Client.Core.Audio;
using Domivium.Client.Core.UI;
using Domivium.Client.Core.UI.Navigation;
using Domivium.Client.Core.UI.Presenter;
using R3;
using UnityEngine;

namespace Domivium.Client.Contents.UI.Static
{
    public class StageStaticUIPresenter : StaticUIPresenter<StageStaticUIView, IStageStaticUIMessage>, IStageStaticUIMessage
    {
        private readonly SceneService _sceneService;
        private readonly ICameraSystemModel _cameraSystemModel;

        protected override HashSet<UILayer> Layer => UILayer.Set(UILayers.Stage);

        public override UIPriority Priority => UIPriorities.Stage;

        public StageStaticUIPresenter(
            StageStaticUIView view,
            IUINavigation navigation,
            IAudioPlayer audioPlayer,
            SceneService sceneService,
            ICharacterSystemModel characterSystemModel)
            : base(view, navigation, audioPlayer)
        {
            _sceneService = sceneService;
            characterSystemModel.OnLookAt.Subscribe(OnLookAt).AddTo(ref DisposableBag);
            characterSystemModel.OnAvoid.Subscribe(OnAvoid).AddTo(ref DisposableBag);
        }

        public void EnterLobby()
        {
            _sceneService.Load(SceneScopeIds.Lobby);
        }

        private void OnLookAt(Vector2 value)
        {
            View.SetAttackButton(value.sqrMagnitude > Constant.CanAttackRange);
        }

        private void OnAvoid(float cooldown)
        {
            View.SetAvoidButton(cooldown);
        }
    }
}