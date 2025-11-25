using Domivium.Client.Core.Message;
using MessagePipe;

namespace Domivium.Client.Core.Battle
{
    public class BattleAbilitySpec
    {
        private readonly BattleAbility _ability;
        private readonly IPublisher<BattleCueMessage> _cuePublisher;
        private float _remainCooldown;

        public BattleAbilityId Id => _ability.Id;
        public float Cooldown { get; private set; }

        public BattleAbilitySpec(BattleAbility ability, IPublisher<BattleCueMessage> cuePublisher)
        {
            _ability = ability;
            _cuePublisher = cuePublisher;
        }

        public bool CanActivateAbility(IBattleSystem battleSystem)
        {
            if (_remainCooldown > 0f) return false;

            return _ability.CanActivate(battleSystem);
        }

        public bool TryActivate(ref BattleAbilityContext context)
        {
            var source = context.Source;
            if (!CanActivateAbility(source)) return false;

            var cooldown = _ability.Activate(ref context);
            Cooldown = cooldown;
            _remainCooldown = cooldown;
            var cueContext = BattleCueContext.Create(source.ActorId, source.Position, source.Direction.CurrentValue);
            _cuePublisher.Publish(BattleCueMessage.Emit(_ability.CueId, cueContext));
            return true;
        }

        public void Tick(float deltaTime)
        {
            if (_remainCooldown > 0f)
            {
                _remainCooldown -= deltaTime;
            }
        }
    }
}