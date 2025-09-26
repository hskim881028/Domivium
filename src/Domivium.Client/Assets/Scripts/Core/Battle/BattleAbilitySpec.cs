using Domivium.Client.Core.Message;
using Domivium.Client.Data.Stat;
using MessagePipe;

namespace Domivium.Client.Core.Battle
{
    public class BattleAbilitySpec
    {
        private readonly BattleAbility _ability;
        private readonly StatSet _stat;
        private readonly IPublisher<BattleCueMessage> _cuePublisher;
        private float _cooldown;

        public BattleAbilityId Id => _ability.Id;

        public BattleAbilitySpec(BattleAbility ability, StatSet stat, IPublisher<BattleCueMessage> cuePublisher)
        {
            _ability = ability;
            _stat = stat;
            _cuePublisher = cuePublisher;
            _cooldown = _ability.ApplyAttackSpeed ? _ability.Cooldown / _stat.RateValue(StatId.AttackSpeed) : _ability.Cooldown;
        }

        public bool TryActivate(ref BattleAbilityContext context)
        {
            if (_cooldown > 0f) return false;

            if (!_ability.TryActivate(ref context)) return false;

            _cooldown = _ability.ApplyAttackSpeed ? _ability.Cooldown / _stat.RateValue(StatId.AttackSpeed) : _ability.Cooldown;
            _cuePublisher.Publish(BattleCueMessage.Emit(_ability.CueId, BattleCueContext.Create(context)));
            return true;
        }

        public void Tick(float deltaTime)
        {
            if (_cooldown > 0f)
            {
                _cooldown -= deltaTime;
            }
        }
    }
}