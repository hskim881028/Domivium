using System;
using System.Collections.Generic;
using Domivium.Client.Core.Actors;

namespace Domivium.Client.Contents.Actors.Generated
{
    public static class ActorMapping
    {
        public static readonly Dictionary<ActorId, (Type presenter, Type view)> Actor = new()
        {
            { ActorIds.Map, (typeof(StageMapPresenter), typeof(StageMap)) },
            { ActorIds.Nexus, (typeof(NexusPresenter), typeof(Nexus)) },
            { ActorIds.Tower, (typeof(TowerPresenter), typeof(Tower)) },
            { ActorIds.Character, (typeof(CharacterPresenter), typeof(Character)) },
            { ActorIds.Monster, (typeof(MonsterPresenter), typeof(Monster)) },
            { ActorIds.CharacterPathIndicator, (typeof(CharacterPathIndicatorPresenter), typeof(CharacterPathIndicator)) },
            { ActorIds.DamageText, (typeof(DamageTextPresenter), typeof(DamageText)) }
        };
    }
}