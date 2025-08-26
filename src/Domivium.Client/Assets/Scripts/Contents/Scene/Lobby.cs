using Domivium.Client.Core.Scene;
using Domivium.Client.Core.Utility;
using VContainer;

namespace Domivium.Client.Contents.Scene
{
    public class Lobby : SceneScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
            base.Configure(builder);
            this.Log();
        }
    }
}