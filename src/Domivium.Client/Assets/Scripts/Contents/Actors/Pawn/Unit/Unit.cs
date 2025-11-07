using System.Threading;
using Cysharp.Threading.Tasks;
using Domivium.Client.Contents.Actors.Generated;
using Domivium.Client.Core.Actors.Contract;
using Domivium.Client.Core.Utility;
using UnityEngine;

namespace Domivium.Client.Contents.Actors
{
    public abstract class Unit : Pawn
    {
        [SerializeField] private GameObject _aim;
        [SerializeField] private LineRenderer _lineRenderer;

        private int _layerMask;

        public override async UniTask SpawnAsync(CancellationToken token, ActorParam param)
        {
            await base.SpawnAsync(token, param);
            _layerMask = ActorId == ActorIds.Character ? Layer.MonsterOrPropMask : Layer.CharacterOrPropMask;
        }

        public void SetAim(Vector2 direction, float range)
        {
            _aim.SetActive(true);

            var hit = Physics2D.Raycast(Muzzle.position, direction, range, _layerMask);
            if (hit.collider != null)
            {
                range = Vector2.Distance(Muzzle.position, hit.point);
            }

            var angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg + 180f;
            Muzzle.rotation = Quaternion.Euler(0f, 0f, angle);

            _lineRenderer.SetPosition(0, Vector3.zero);
            _lineRenderer.SetPosition(1, Vector3.left * range);
        }

        public void HideAim()
        {
            _aim.SetActive(false);
        }
    }
}