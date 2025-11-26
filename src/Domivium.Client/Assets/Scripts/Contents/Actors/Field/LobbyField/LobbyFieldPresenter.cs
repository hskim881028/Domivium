using Domivium.Client.Core.Factory;

namespace Domivium.Client.Contents.Actors
{
    public class LobbyFieldPresenter : FieldPresenter<LobbyField>
    {
        public LobbyFieldPresenter(LobbyField actor, ISystemFactory systemFactory) : base(actor, systemFactory) { }
    }
}