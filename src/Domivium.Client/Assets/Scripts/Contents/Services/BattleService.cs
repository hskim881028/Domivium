using Domivium.Client.Contents.Commands;
using Domivium.Client.Contents.ReadModels;
using Domivium.Client.Core.Director;

namespace Domivium.Client.Contents.Services
{
    public sealed class BattleService : IBattleReadModel, IBattleCommand
    {
        private readonly IStageDirector _director;

        public BattleService(IStageDirector director)
        {
            _director = director;
        }
    }
}