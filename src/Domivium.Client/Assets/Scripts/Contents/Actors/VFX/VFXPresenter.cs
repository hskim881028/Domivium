using System;
using Domivium.Client.Core.Actors;
using Domivium.Client.Core.Message;
using MessagePipe;

namespace Domivium.Client.Contents.Actors
{
    public abstract class VFXPresenter<TVFX> : ActorPresenter<TVFX>, IVFXPresenter where TVFX : VFX
    {
        protected VFXPresenter(Guid id, TVFX actor, IPublisher<ActorTagMessage> tagPublisher)
            : base(id, actor, tagPublisher) { }

        public virtual void Tick(float deltaTime)
        {
            Actor.Tick(deltaTime);
        }
    }
}