using System;
using System.Collections.Generic;
using Domivium.Client.Contents.Battle.Effect;
using Domivium.Client.Core.Battle;
using Domivium.Client.Core.Message;
using MessagePipe;
using R3;

namespace Domivium.Client.Contents.Battle
{
    public sealed class BattleEffectPool : Disposable, IBattleEffectPool
    {
        private readonly IPublisher<BattleCueMessage> _cuePublisher;
        private readonly Dictionary<BattleEffectId, Queue<BattleEffectSpec>> _specs = new();

        public BattleEffectPool(ISubscriber<SceneMessage> sceneSubscriber, IPublisher<BattleCueMessage> cuePublisher)
        {
            sceneSubscriber.Subscribe(OnSceneMessage).AddTo(ref DisposableBag);
            _cuePublisher = cuePublisher;
        }

        public BattleEffectSpec Get(BattleEffectId id, BattleAbilityContext abilityContext, BattleAbility ability)
        {
            var context = BattleEffectContext.Create(abilityContext, ability);
            if (!_specs.ContainsKey(id))
            {
                _specs[id] = new Queue<BattleEffectSpec>();
            }

            if (_specs[id].Count > 0)
            {
                var spec = _specs[id].Dequeue();
                spec.Reset(ref context);
                return spec;
            }

            var effect = CreateEffect(id, ref context);
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

        private static BattleEffect CreateEffect(BattleEffectId id, ref BattleEffectContext context)
        {
            if (id == BattleEffectIds.Damage)
            {
                return new DamageEffect(ref context);
            }

            throw new Exception($"Invalid battle effect: {id}");
        }

        private void OnSceneMessage(SceneMessage message)
        {
            switch (message.Type)
            {
                case SceneMessageType.Unload:
                case SceneMessageType.Load:
                    _specs.Clear();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}