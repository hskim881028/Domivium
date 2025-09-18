using Domivium.Client.Core.Message;
using MessagePipe;

namespace Domivium.Client.Core.Battle
{
    public class BattleAbilitySpec
    {
        private readonly BattleAbility _ability;
        private readonly IPublisher<BattleCueMessage> _cuePublisher;
        private float _cooldown;

        public BattleAbilityId Id => _ability.Id;

        public BattleAbilitySpec(BattleAbility ability, IPublisher<BattleCueMessage> cuePublisher)
        {
            _ability = ability;
            _cuePublisher = cuePublisher;
            _cooldown = ability.Cooldown;
        }

        public bool TryActivate(BattleSystem target)
        {
            if (_cooldown > 0f) return false;

            var context = target.CreateBattleContext();
            if (!_ability.TryActivate(target, ref context)) return false;

            _cooldown = _ability.Cooldown;
            _cuePublisher.Publish(BattleCueMessage.Emit(_ability.CueId, in context));
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