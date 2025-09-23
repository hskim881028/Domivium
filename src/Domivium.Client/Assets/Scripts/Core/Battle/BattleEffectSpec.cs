using System;
using System.Collections.Generic;
using Domivium.Client.Core.Message;
using MessagePipe;

namespace Domivium.Client.Core.Battle
{
    public class BattleEffectSpec
    {
        private readonly BattleEffect _effect;
        private readonly IPublisher<BattleCueMessage> _cuePublisher;
        private readonly Action<BattleEffectSpec> _onDeactivate;
        private float _duration;
        private float _periodicInterval;

        public BattleEffectId Id => _effect.Id;
        public BattleEffectContext Context => _effect.Context;
        public IReadOnlyCollection<BattleTag> GrantedTags => _effect.GrantedBattleTags;
        public IReadOnlyList<BattleStatModifier> StatModifiers => _effect.StatModifiers;
        public IReadOnlyList<BattleStatModifier> StatPeriodicModifiers => _effect.StatPeriodicModifiers;
        public IReadOnlyList<BattleGaugeModifier> GaugeModifiers => _effect.GaugeModifiers;
        public IReadOnlyList<BattleGaugeModifier> GaugePeriodicModifiers => _effect.GaugePeriodicModifiers;

        public BattleEffectSpec(
            BattleEffect effect,
            IPublisher<BattleCueMessage> cuePublisher,
            Action<BattleEffectSpec> onDeactivate)
        {
            _effect = effect;
            _cuePublisher = cuePublisher;
            _onDeactivate = onDeactivate;
            ResetInternal(_effect);
        }

        public void Reset(ref BattleEffectContext context)
        {
            _effect.Reset(context);
            ResetInternal(_effect);
        }

        public bool Tick(float deltaTime)
        {
            _duration -= deltaTime;
            return _duration >= 0;
        }

        public bool TryActivate(BattleSystem target)
        {
            if (!_effect.TryActivate(target))
            {
                _onDeactivate.Invoke(this);
                return false;
            }

            _cuePublisher.Publish(BattleCueMessage.Emit(_effect.CueId, BattleCueContext.Create(_effect.Context)));
            return true;
        }

        public bool TryActivateForPeriodic(float deltaTime)
        {
            _periodicInterval -= deltaTime;
            if (_periodicInterval > 0) return false;

            _periodicInterval = _effect.PeriodicInterval;
            _cuePublisher.Publish(BattleCueMessage.Emit(_effect.PeriodicCueId, BattleCueContext.Create(_effect.Context)));
            return true;
        }

        public void Deactivate(bool hideCue = false)
        {
            _cuePublisher.Publish(BattleCueMessage.Emit(_effect.DeactivateCueId, BattleCueContext.Create(_effect.Context)));
            _onDeactivate.Invoke(this);
        }

        private void ResetInternal(BattleEffect effect)
        {
            _duration = effect.Duration;
            _periodicInterval = effect.PeriodicInterval;
        }
    }
}