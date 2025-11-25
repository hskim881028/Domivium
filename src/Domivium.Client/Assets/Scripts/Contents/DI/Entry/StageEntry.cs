using Domivium.Client.Core.Battle;
using Domivium.Client.Core.Input;
using Domivium.Client.Core.Systems;
using Domivium.Client.Core.UI.Navigation;
using Domivium.Client.Data.Config;
using UnityEngine;
using VContainer.Unity;

namespace Domivium.Client.Contents.DI.Entry
{
    public class StageEntry : Entry, ITickable
    {
        private readonly IStageSystemCommand _stageSystemCommand;

        public StageEntry(
            IUINavigation uiNavigation,
            IInputComposition inputComposition,
            IBattleEffectPool effectPool,
            IBattleCuePlayer cuePlayer,
            IStageSystemCommand stageSystemCommand) : base(uiNavigation)
        {
            _stageSystemCommand = stageSystemCommand;
        }

        protected override void OnStart()
        {
            var config = new StageConfig { StageId = 1 };
            _stageSystemCommand.RunAsync(config.StageId).Forget();
        }

        public void Tick()
        {
            _stageSystemCommand.Tick(Time.deltaTime);
        }
    }
}