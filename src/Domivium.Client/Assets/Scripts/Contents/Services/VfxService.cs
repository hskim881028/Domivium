using System;
using System.Collections.Generic;
using Domivium.Client.Contents.Actors.Contract;
using Domivium.Client.Contents.Actors.Generated;
using Domivium.Client.Contents.Battle;
using Domivium.Client.Core.Actors;
using Domivium.Client.Core.Battle;
using Domivium.Client.Core.Message;
using MessagePipe;
using R3;

namespace Domivium.Client.Contents.Services
{
    public class VfxService : Disposable
    {
        private readonly IActorSpawner _actorSpawner;
        private readonly Dictionary<Guid, IVFXPresenter> _vfx = new();

        public VfxService(IActorSpawner actorSpawner)
        {
            _actorSpawner = actorSpawner;
        }

        public void Spawn(BattleCueId cueId, in BattleContext context)
        {
            if (cueId == BattleCueIds.Damaged)
            {
                _actorSpawner.SpawnAsync(ActorIds.DamageText, new DamageTextParams(context.Unit.position, context.Damage, 1.6f));
            }
        }
    }
}