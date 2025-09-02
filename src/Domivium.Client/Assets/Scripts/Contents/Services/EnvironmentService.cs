using Domivium.Client.Core.Actors;

namespace Domivium.Client.Contents.Services
{
    public class EnvironmentService
    {
        private readonly EnvironmentRig _environmentRig;

        public EnvironmentService(EnvironmentRig environmentRig)
        {
            _environmentRig = environmentRig;
        }
    }
}