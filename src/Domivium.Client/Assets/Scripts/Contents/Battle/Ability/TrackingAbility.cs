using System;
using Domivium.Client.Contents.Actors.Generated;
using Domivium.Client.Core.Actors;
using Domivium.Client.Core.Battle;
using Domivium.Client.Core.Utility;
using Domivium.Client.Data.Stat;
using UnityEngine;

namespace Domivium.Client.Contents.Battle.Ability
{
    public class TrackingAbility : BattleAbility
    {
        private readonly IActorManager _actorManager;
        private readonly RaycastHit2D[] _hits = new RaycastHit2D[8];

        public override BattleAbilityId Id => BattleAbilityIds.Tracking;

        public TrackingAbility(IBattleEffectPool effectPool, IActorManager actorManager) : base(effectPool)
        {
            _actorManager = actorManager;
        }

        public override float Activate(ref BattleAbilityContext context)
        {
            var source = context.Source;
            var pre = source.PrePosition;
            var cur = source.Position;
            var dist = Vector3.Distance(pre, cur);
            
            if (Mathf.Approximately(dist, 0))
            {
                return Move(source, context.DeltaTime);
            }

            // var hitRange = source.Stat.RateValue(StatId.HitRange);
            // todo: 범위 공격 생기면 구현 필요.

            var dir = source.Direction.CurrentValue;
            var mask = context.Target == ActorId.Character ? Layer.CharacterOrPropMask : Layer.MonsterOrPropMask;
            var count = Physics2D.RaycastNonAlloc(pre, dir, _hits, dist, mask);
            if (count <= 0)
            {
                return Move(source, context.DeltaTime);
            }

            Array.Sort(_hits, 0, count, HitDistanceComparer.Instance);
            var durability = source.Gauge.Current(StatId.Durability);
            for (var i = 0; i < count; i++)
            {
                if (durability <= 0) break;
                
                var actor = _hits[i].transform.GetComponent<Actor>();
                if (!context.Source.TryAddHistory(actor.Uid)) continue;

                var durabilityEffect = EffectPool.Get(BattleEffectIds.Durability, context.Source, context.Source);
                context.Source.ActivateEffect(durabilityEffect);

                if (actor.Id == context.Target)
                {
                    if (_actorManager.TryGetPawn(actor.Id, actor.Uid, out var pawn))
                    {
                        var effect = EffectPool.Get(BattleEffectIds.Damage, context.Source, pawn);
                        pawn.ActivateEffect(effect);
                    }
                }

                durability--;
            }

            return Move(source, context.DeltaTime);
        }

        private float Move(IBattleSystem source, float deltaTime)
        {
            var speed = source.Stat.RateValue(StatId.MoveSpeed);
            var position = source.Position;
            var direction = source.Direction.CurrentValue;
            direction *= speed * deltaTime;
            source.SetPosition(position + direction);
            return 0;
        }
    }
}