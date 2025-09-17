using Domivium.Client.Core.Actors;

namespace Domivium.Client.Contents.Actors
{
    public abstract class VFX : Actor, IVFX
    {
        public virtual void Tick(float deltaTime) { }
    }
}