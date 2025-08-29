using UnityEngine.SceneManagement;
using VContainer;
using VContainer.Unity;

namespace Domivium.Client.Core.Scene
{
    public class ActorRootScope : LifetimeScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
            base.Configure(builder);
            transform.SetParent(null);
            SceneManager.MoveGameObjectToScene(gameObject, SceneManager.GetActiveScene());
        }
    }
}