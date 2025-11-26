using System;
using System.Collections.Generic;
using Domivium.Client.Core.Actors;

namespace Domivium.Client.Contents.Actors.Generated
{
    public static class ActorMapping
    {
        public static readonly Dictionary<ActorId, (Type presenter, Type view)> Actor = new()
        {
            { ActorId.Character, (typeof(CharacterPresenter), typeof(Character)) },
            { ActorId.Monster, (typeof(MonsterPresenter), typeof(Monster)) },
            { ActorId.Projectile, (typeof(ProjectilePresenter), typeof(Projectile)) },
            { ActorId.Prop, (typeof(PropPresenter), typeof(Prop)) },

            //Field
            { ActorId.LobbyField, (typeof(LobbyFieldPresenter), typeof(LobbyField)) },
            { ActorId.StageField, (typeof(StageFieldPresenter), typeof(StageField)) },

            // VFX
            { ActorId.DamageText, (typeof(DamageTextPresenter), typeof(DamageText)) },
            { ActorId.HealText, (typeof(HealTextPresenter), typeof(HealText)) },
        };
    }
}