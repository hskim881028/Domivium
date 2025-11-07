using System.Threading;
using Cysharp.Threading.Tasks;
using Domivium.Client.Contents.UIComponents;
using Domivium.Client.Core.Component;
using Domivium.Client.Core.UI.View;
using UnityEngine;
using UnityEngine.UI;

namespace Domivium.Client.Contents.UI.Static
{
    public class StageStaticUIView : StaticUIView<IStageStaticUIMessage>
    {
        [SerializeField] private DvmButton _enterLobbyButton;
        [SerializeField] private Image[] _rightStickImages;
        [SerializeField] private FilledOnScreenButton _avoidButton;

        [SerializeField] private StatGauge _healthGauge;
        [SerializeField] private StatGauge _hungerGauge;
        [SerializeField] private StatGauge _staminaGauge;
        [SerializeField] private StatGauge _sanityGauge;
        [SerializeField] private ProjectileCapacity _projectileCapacity;

        public override async UniTask InitializeAsync(CancellationToken token)
        {
            _enterLobbyButton.onClick.AddListener(Message.EnterLobby);
            await base.InitializeAsync(token);
        }

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

        public void SetHealth(int cur, int max)
        {
            _healthGauge.Set(cur, max);
        }

        public void SetHunger(int cur, int max)
        {
            _hungerGauge.Set(cur, max);
        }

        public void SetStamina(int cur, int max)
        {
            _staminaGauge.Set(cur, max);
        }

        public void SetSanity(int cur, int max)
        {
            _sanityGauge.Set(cur, max);
        }

        public void SetProjectileCapacity(int cur, int max)
        {
            _projectileCapacity.Set(cur, max);
        }

        public void Reload(float duration)
        {
            _projectileCapacity.Reload(duration).Forget();
        }

        public void CancelReload()
        {
            _projectileCapacity.CancelReload();
        }
    }
}