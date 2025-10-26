using System;
using System.Collections.Generic;
using Domivium.Client.Core.Actors;

namespace Domivium.Client.Contents.Actors.Generated
{
    public static class ActorMapping
    {
        public static readonly Dictionary<ActorId, (Type presenter, Type view)> Actor = new()
        {
            { ActorIds.Map, (typeof(StageFieldPresenter), typeof(StageField)) },
            { ActorIds.MonsterCamp, (typeof(MonsterCampPresenter), typeof(MonsterCamp)) },
            { ActorIds.Nexus, (typeof(NexusPresenter), typeof(Nexus)) },
            { ActorIds.Tower, (typeof(TowerPresenter), typeof(Tower)) },
            { ActorIds.Character, (typeof(CharacterPresenter), typeof(Character)) },
            { ActorIds.Monster, (typeof(MonsterPresenter), typeof(Monster)) },
            { ActorIds.CharacterPathIndicator, (typeof(CharacterPathIndicatorPresenter), typeof(CharacterPathIndicator)) },
            { ActorIds.CharacterSelectIndicator, (typeof(CharacterSelectIndicatorPresenter), typeof(CharacterSelectIndicator)) },
            { ActorIds.DamageText, (typeof(DamageTextPresenter), typeof(DamageText)) },
            { ActorIds.HealText, (typeof(HealTextPresenter), typeof(HealText)) },
            { ActorIds.SoulEffect, (typeof(SoulEffectPresenter), typeof(SoulEffect)) },
            { ActorIds.SlashEffect, (typeof(SlashEffectPresenter), typeof(SlashEffect)) },
            { ActorIds.ProjectileEffect, (typeof(ProjectileEffectPresenter), typeof(ProjectileEffect)) }
        };
    }
}