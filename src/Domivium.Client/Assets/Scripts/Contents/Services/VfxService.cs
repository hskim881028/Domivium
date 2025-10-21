using Domivium.Client.Contents.Actors.Contract;
using Domivium.Client.Contents.Actors.Generated;
using Domivium.Client.Contents.Battle;
using Domivium.Client.Core.Actors;
using Domivium.Client.Core.Battle;

namespace Domivium.Client.Contents.Services
{
    public sealed class VfxService
    {
        private readonly IActorSpawner _actorSpawner;

        public VfxService(IActorSpawner actorSpawner)
        {
            _actorSpawner = actorSpawner;
        }

        public void Spawn(BattleCueId cueId, in BattleCueContext context)
        {
            if (cueId == BattleCueIds.Damaged)
            {
                _actorSpawner.SpawnAsync(ActorIds.DamageText, new DamageTextParams(context.Position, context.Value, 1.6f));
            }

            if (cueId == BattleCueIds.Healed)
            {
                _actorSpawner.SpawnAsync(ActorIds.HealText, new HealTextParams(context.Position, context.Value, 1.6f));
            }

            if (cueId == BattleCueIds.Attack)
            {
                _actorSpawner.SpawnAsync(ActorIds.SlashEffect,
                    new SlashEffectParams(
                        context.ActorId,
                        context.Position,
                        context.Direction,
                        context.Value,
                        1.6f));
            }
        }
    }
}