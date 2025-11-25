using Domivium.Client.Contents.UIComponents;
using Domivium.Client.Core.Component;
using Domivium.Client.Core.Component.Stat;
using Domivium.Client.Core.UI.View;
using UnityEngine;
using UnityEngine.UI;

namespace Domivium.Client.Contents.UI.Static
{
    public class StageStaticUIView : StaticUIView<IStageStaticUIMessage>
    {
        [SerializeField] private RectTransform _attackStick;
        [SerializeField] private Image[] _rightStickImages;
        [SerializeField] private FilledOnScreenButton _avoidButton;
        [SerializeField] private GameObject _interactButton;
        [SerializeField] private GaugeStat _health;
        [SerializeField] private GaugeStat _hunger;
        [SerializeField] private GaugeStat _stamina;
        [SerializeField] private GaugeStat _sanity;
        [SerializeField] private ProjectileCapacity _projectileCapacity;

        public void SetAttackButton(bool canAttack)
        {
            var color = canAttack ? Constant.ActiveAttackButtonColor : Color.white;
            foreach (var image in _rightStickImages)
            {
                image.color = color;
            }
        }

        public void SetAvoidButton(float cooldown)
        {
            _avoidButton.SetAsync(cooldown).Forget();
        }

        public void SetInteractButton(bool value)
        {
            _interactButton.SetActive(value);
        }

        public void SetHealth(int cur, int max)
        {
            _health.Set(cur, max);
        }

        public void SetHunger(int cur, int max)
        {
            _hunger.Set(cur, max);
        }

        public void SetStamina(int cur, int max)
        {
            _stamina.Set(cur, max);
        }

        public void SetSanity(int cur, int max)
        {
            _sanity.Set(cur, max);
        }

        public void SetProjectileCapacity(int cur, int max)
        {
            _projectileCapacity.Set(cur, max);
        }

        public void Reload(float duration)
        {
            _attackStick.anchoredPosition = Vector2.zero;
            _projectileCapacity.Reload(duration).Forget();
        }

        public void CancelReload()
        {
            _projectileCapacity.CancelReload();
        }
    }
}