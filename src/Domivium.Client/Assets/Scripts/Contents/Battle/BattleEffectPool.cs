using System;
using System.Collections.Generic;
using Domivium.Client.Contents.Battle.Effect;
using Domivium.Client.Core.Battle;
using Domivium.Client.Core.Message;
using MessagePipe;

namespace Domivium.Client.Contents.Battle
{
    public sealed class BattleEffectPool : IBattleEffectPool
    {
        private readonly IPublisher<BattleCueMessage> _cuePublisher;
        private readonly Dictionary<BattleEffectId, Queue<BattleEffectSpec>> _specs = new();

        public BattleEffectPool(IPublisher<BattleCueMessage> cuePublisher)
        {
            _cuePublisher = cuePublisher;
        }

        public BattleEffectSpec Get(BattleEffectId id, in BattleContext context)
        {
            if (!_specs.ContainsKey(id))
            {
                _specs[id] = new Queue<BattleEffectSpec>();
            }

            if (_specs[id].Count > 0)
            {
                var spec = _specs[id].Dequeue();
                spec.Reset(context);
                return spec;
            }

            var effect = CreateEffect(id, context);
            return new BattleEffectSpec(effect, _cuePublisher, Return);
        }

        private void Return(BattleEffectSpec spec)
        {
            if (!_specs.ContainsKey(spec.Id))
            {
                _specs[spec.Id] = new Queue<BattleEffectSpec>();
            }

            _specs[spec.Id].Enqueue(spec);
        }

        private static BattleEffect CreateEffect(BattleEffectId id, in BattleContext context)
        {
            if (id == BattleEffectIds.Damage)
            {
                return new DamageEffect(context);
            }

            throw new Exception($"Invalid battle effect: {id}");
        }
    }
}