using UnityEngine.Rendering.Universal;
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
            gameObject.AddComponent<CompositeShadowCaster2D>();
            SceneManager.MoveGameObjectToScene(gameObject, SceneManager.GetActiveScene());
        }
    }
}