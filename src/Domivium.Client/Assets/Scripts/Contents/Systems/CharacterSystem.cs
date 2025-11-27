using Domivium.Client.Contents.Battle;
using Domivium.Client.Core;
using Domivium.Client.Core.Battle;
using Domivium.Client.Core.Systems;
using R3;
using UnityEngine;

namespace Domivium.Client.Contents.Systems
{
    public sealed class CharacterSystem : Disposable, ICharacterSystem, ICharacterSystemCommand
    {
        private readonly ReactiveProperty<Vector2> _direction = new();
        private readonly ReactiveProperty<Vector2> _lookAt = new();

        public ReactiveCommand<BattleTag> OnBattleTag { get; } = new();
        public ReadOnlyReactiveProperty<Vector2> OnTurn => _direction;
        public ReadOnlyReactiveProperty<Vector2> OnLookAt => _lookAt;

        public CharacterSystem(IAppContext appContext)
        {
            appContext.Mode.Subscribe(OnChangeMode).AddTo(ref DisposableBag);
        }

        public void Stop()
        {
            _direction.Value = Vector2.zero;
            _lookAt.Value = Vector2.zero;
            OnBattleTag.Execute(BattleTags.Idle);
        }

        public bool SetDirection(Vector2 value)
        {
            if (value.sqrMagnitude > 1f)
            {
                value.Normalize();
            }

            _direction.Value = value;
            return true;
        }

        public bool LookAt(Vector2 value)
        {
            _lookAt.Value = value;
            switch (_lookAt.Value.sqrMagnitude)
            {
                case > 0 when value.sqrMagnitude <= Constant.CanAttackRange:
                    OnBattleTag.Execute(BattleTags.Aiming);
                    break;
                case > Constant.CanAttackRange:
                    OnBattleTag.Execute(BattleTags.Firing);
                    break;
                default:
                    OnBattleTag.Execute(BattleTags.Idle);
                    break;
            }

            return true;
        }

        public bool Avoid()
        {
            OnBattleTag.Execute(BattleTags.Avoid);
            return true;
        }

        private void OnChangeMode(StageMode mode)
        {
            if (mode == StageMode.Run)
            {
                Stop();
            }
        }
    }
}