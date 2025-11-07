using System;
using System.Collections.Generic;
using Domivium.Client.Core.Actors;

namespace Domivium.Client.Contents.Actors.Generated
{
    public static class ActorMapping
    {
        public static readonly Dictionary<ActorId, (Type presenter, Type view)> Actor = new()
        {
            { ActorIds.StageField, (typeof(StageFieldPresenter), typeof(StageField)) },
            { ActorIds.Character, (typeof(CharacterPresenter), typeof(Character)) },
            { ActorIds.Monster, (typeof(MonsterPresenter), typeof(Monster)) },
            { ActorIds.Projectile, (typeof(ProjectilePresenter), typeof(Projectile)) },
            
            // VFX
            { ActorIds.DamageText, (typeof(DamageTextPresenter), typeof(DamageText)) },
            { ActorIds.HealText, (typeof(HealTextPresenter), typeof(HealText)) },

            // Prop
            { ActorIds.Prop, (typeof(PropPresenter), typeof(Prop)) },
        };
    }
}