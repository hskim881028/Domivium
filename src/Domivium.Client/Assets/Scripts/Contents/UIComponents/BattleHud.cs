using Domivium.Client.Core.Component;
using UnityEngine;
using UnityEngine.UI;

namespace Domivium.Client.Contents.UIComponents
{
    public class BattleHud : MonoBehaviour
    {
        [SerializeField] private RectTransform _attackStick;
        [SerializeField] private Image[] _attackImages;
        [SerializeField] private FilledOnScreenButton _avoidButton;
        [SerializeField] private GameObject _interactButton;
        [SerializeField] private ProjectileCapacity _projectileCapacity;

        public void SetAttackButton(bool canAttack)
        {
            var color = canAttack ? Constant.ActiveAttackButtonColor : Color.white;
            foreach (var image in _attackImages)
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