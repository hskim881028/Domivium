using System;
using System.Collections.Generic;
using Domivium.Client.Core.Battle;
using Domivium.Client.Core.Factory;
using Domivium.Client.Core.Message;
using MessagePipe;
using R3;

namespace Domivium.Client.Contents.Battle
{
    public sealed class BattleEffectPool : Disposable, IBattleEffectPool
    {
        private readonly IBattleEffectFactory _effectFactory;
        private readonly IPublisher<BattleCueMessage> _cuePublisher;
        private readonly Dictionary<BattleEffectId, Queue<BattleEffectSpec>> _specs = new();

        public BattleEffectPool(
            IBattleEffectFactory effectFactory,
            IPublisher<BattleCueMessage> cuePublisher,
            ISubscriber<SceneMessage> sceneSubscriber)
        {
            _effectFactory = effectFactory;
            _cuePublisher = cuePublisher;
            sceneSubscriber.Subscribe(OnSceneMessage).AddTo(ref DisposableBag);
        }

        public BattleEffectSpec Get(BattleEffectId id, IBattleSystem source, IBattleSystem owner)
        {
            var context = BattleEffectContext.Create(id, source, owner);
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

            var effect = _effectFactory.Create(id, ref context);
            var effectSpec = new BattleEffectSpec(effect, _cuePublisher, Return);
            effectSpec.Reset(ref context);
            return effectSpec;
        }

        private void Return(BattleEffectSpec spec)
        {
            if (!_specs.ContainsKey(spec.Id))
            {
                _specs[spec.Id] = new Queue<BattleEffectSpec>();
            }

            _specs[spec.Id].Enqueue(spec);
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