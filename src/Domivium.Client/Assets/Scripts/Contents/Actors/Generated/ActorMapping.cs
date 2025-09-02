using System;
using System.Collections.Generic;
using Domivium.Client.Contents.Actors.Character;
using Domivium.Client.Contents.Actors.StageMap;
using Domivium.Client.Core.Actors;

namespace Domivium.Client.Contents.Actors.Generated
{
    public static class ActorMapping
    {
        public static readonly Dictionary<ActorId, (Type presenter, Type view)> Actor = new()
        {
            { ActorIds.Map, (typeof(StageMapPresenter), typeof(StageMap.StageMap)) },
            { ActorIds.Character, (typeof(CharacterPresenter), typeof(Character.Character)) }
        };
    }
}