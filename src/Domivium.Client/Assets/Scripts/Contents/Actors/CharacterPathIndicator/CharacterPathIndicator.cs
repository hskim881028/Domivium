using System.Threading;
using Cysharp.Threading.Tasks;
using Domivium.Client.Core.Actors;
using Domivium.Client.Core.Actors.Contract;
using Domivium.Client.Core.Battle;
using UnityEngine;

namespace Domivium.Client.Contents.Actors
{
    public class CharacterPathIndicator : Actor
    {
        [SerializeField] private LineRenderer _lineRenderer;

        private IBattleSystem _character;
        private Vector3 _targetPosition;
        private bool _isPicked;

        public override UniTask ActivateAsync(CancellationToken token, ActorParam param)
        {
            Release();
            return base.ActivateAsync(token, param);
        }

        public void SetCharacter(IBattleSystem character)
        {
            _character = character;
            _isPicked = _character != null;
            if (!_isPicked)
            {
                Release();
            }
        }

        public void SetTargetPosition(Vector3 position)
        {
            _targetPosition = position;
            _targetPosition.y = 0.1f;
        }

        private void Update()
        {
            if (!_isPicked) return;

            var position = _character.UnitPosition;
            position.y = 0.1f;
            _lineRenderer.SetPosition(0, position);
            _lineRenderer.SetPosition(1, _targetPosition);
        }

        private void Release()
        {
            _lineRenderer.SetPosition(0, Vector3.zero);
            _lineRenderer.SetPosition(1, Vector3.zero);
        }
    }
}