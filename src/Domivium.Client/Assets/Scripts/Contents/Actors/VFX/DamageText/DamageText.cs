using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Domivium.Client.Contents.Actors.Contract;
using Domivium.Client.Core.Actors.Contract;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Domivium.Client.Contents.Actors
{
    public class DamageText : VFX
    {
        [SerializeField] private Transform _axis;
        [SerializeField] private TextMeshPro _text;

        private const string Zero = "0";
        private Image _image;
        private Sequence _punch;
        private Sequence _shake;
        private Color _color;


        public override void Initialize(Transform parent)
        {
            _color = _text.color;

            _punch = DOTween.Sequence();
            _punch.Append(transform.DOPunchPosition(new Vector3(0, 0.8f, 0), 0.3f, vibrato: 1, elasticity: 0.8f));
            _punch.Append(transform.DOPunchPosition(new Vector3(0, 0.4f, 0), 0.3f, vibrato: 2, elasticity: 0.8f));
            _punch.Append(transform.DOPunchPosition(new Vector3(0, 0.2f, 0), 0.4f, vibrato: 3, elasticity: 0.5f));
            _punch.Join(_text.DOFade(0, 0.5f).SetDelay(0.4f));
            _punch.Pause();
            _punch.SetAutoKill(false);

            _shake = DOTween.Sequence();
            _shake.Append(_axis.DOShakePosition(0.8f, new Vector3(0.05f, 0, 0)));
            _shake.Pause();
            _shake.SetAutoKill(false);
            base.Initialize(parent);
        }

        public override UniTask ActivateAsync(CancellationToken token, ActorParam param)
        {
            var p = param.As<DamageTextParams>();
            var position = p.Position;
            var circle = Random.insideUnitCircle;
            position.x += circle.x * 0.3f;
            position.y += 0.5f + circle.y * 0.1f;
            transform.localPosition = position;
            _text.text = $"{p.Damage}";
            _text.color = _color;
            _punch.Restart();
            _shake.Restart();
            return base.ActivateAsync(token, param);
        }

        public override void Deactivate()
        {
            _punch.Pause();
            _shake.Pause();
            _text.color = Color.clear;
            _text.text = Zero;
            base.Deactivate();
        }
    }
}