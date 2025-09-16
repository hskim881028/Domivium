using System;
using Domivium.Client.Core.Actors;
using Domivium.Client.Core.Scene;

namespace Domivium.Client.Core.Message
{
    public readonly struct SpawnerMessage
    {
        public SpawnerMessageType Type { get; }
        public ActorId ActorId { get; }
        public Guid ScopeId { get; }
        public IActorPresenter Presenter { get; }

        private SpawnerMessage(
            SpawnerMessageType type,
            ActorId actorId,
            Guid scopeId,
            IActorPresenter presenter = null)
        {
            Type = type;
            ActorId = actorId;
            ScopeId = scopeId;
            Presenter = presenter;
        }

        public static SpawnerMessage Spawn(ActorScope scope, IActorPresenter presenter)
            => new(SpawnerMessageType.Spawn, scope.ActorId, scope.ScopeId, presenter);

        public static SpawnerMessage Despawn(ActorScope scope) => new(SpawnerMessageType.Despawn, scope.ActorId, scope.ScopeId);
    }
}