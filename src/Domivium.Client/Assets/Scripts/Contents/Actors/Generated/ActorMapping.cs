using System;
using System.Collections.Generic;
using Domivium.Client.Core.Actors;

namespace Domivium.Client.Contents.Actors.Generated
{
    public static class ActorMapping
    {
        public static readonly Dictionary<ActorId, (Type presenter, Type view)> Actor = new()
        {
            { ActorId.StageField, (typeof(StageFieldPresenter), typeof(StageField)) },
            { ActorId.Character, (typeof(CharacterPresenter), typeof(Character)) },
            { ActorId.Monster, (typeof(MonsterPresenter), typeof(Monster)) },
            { ActorId.Projectile, (typeof(ProjectilePresenter), typeof(Projectile)) },
            
            // VFX
            { ActorId.DamageText, (typeof(DamageTextPresenter), typeof(DamageText)) },
            { ActorId.HealText, (typeof(HealTextPresenter), typeof(HealText)) },

            // Prop
            { ActorId.Prop, (typeof(PropPresenter), typeof(Prop)) },
        };
    }
}