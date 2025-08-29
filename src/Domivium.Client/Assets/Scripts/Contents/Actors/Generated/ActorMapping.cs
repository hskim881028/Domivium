using System;
using System.Collections.Generic;
using Domivium.Client.Core.Actors;

namespace Domivium.Client.Contents.Actors.Generated
{
    public static class ActorMapping
    {
        public static readonly Dictionary<ActorId, (Type presenter, Type view)> Actor = new()
        {
            { ActorIds.Map, (typeof(StageMapPresenter), typeof(StageMap)) }
        };
    }
}