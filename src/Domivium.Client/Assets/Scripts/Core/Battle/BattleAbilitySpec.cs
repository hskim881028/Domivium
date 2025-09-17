using System.Collections.Generic;
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
            if (!CanActivate(target)) return false;

            _cooldown = _ability.Cooldown;

            var context = target.CreateBattleContext(_ability);
            // var context = BattleContext.Create(target, _ability, target.Unit, 0);
            // var targets = ResolveTargets();              // 능력 정의 기반 타겟팅
            // var enemies = TargetingService.GetUnitsInRadius(owner.Position, ability.Radius, TargetType.Enemy);
            _ability.Activate(new List<BattleSystem> { target }, in context);
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

        private bool CanActivate(BattleSystem target)
        {
            if (_cooldown > 0f) return false;
            
            if (!_ability.PassesTagRequirements(target.Tags)) return false;

            // todo: 공격 범위등등 조건 다 체크.

            return true;
        }
    }
}