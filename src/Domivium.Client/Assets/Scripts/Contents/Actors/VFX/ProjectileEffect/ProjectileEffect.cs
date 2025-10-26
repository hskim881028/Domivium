using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Domivium.Client.Contents.Actors.Contract;
using Domivium.Client.Contents.Actors.Generated;
using Domivium.Client.Core.Actors;
using Domivium.Client.Core.Actors.Contract;
using UnityEngine;

namespace Domivium.Client.Contents.Actors
{
    public class ProjectileEffect : VFX
    {
        [SerializeField] private SpriteRenderer _renderer;

        [SerializeField] [ColorUsage(true, true)]
        private Color _allyColor;

        [SerializeField] [ColorUsage(true, true)]
        private Color _monsterColor;

        private readonly Dictionary<ActorId, Color> _colors = new();

        protected override void OnAwake()
        {
            base.OnAwake();
            _colors.Add(ActorIds.Character, _allyColor);
            _colors.Add(ActorIds.Tower, _allyColor);
            _colors.Add(ActorIds.Monster, _monsterColor);
        }

        public override UniTask ActivateAsync(CancellationToken token, ActorParam param)
        {
            var p = param.As<ProjectileEffectParams>();
            if (_colors.TryGetValue(p.ActorId, out var color))
            {
                _renderer.color = color;
            }

            transform.localPosition = new Vector3(p.Position.x, 0.5f, p.Position.z);
            transform.DOMove(new Vector3(p.EndPosition.x, 0.5f, p.EndPosition.z), 0.1f);

            return base.ActivateAsync(token, param);
        }
    }
}