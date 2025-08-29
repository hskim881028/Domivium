using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Domivium.Client.Core.Actors;
using Domivium.Client.Core.Actors.Contract;
using Domivium.Client.Core.Utility;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Domivium.Client.Contents.Actors
{
    public class StageMap : Actor
    {
        [SerializeField] private Tilemap _background;
        [SerializeField] private Tilemap _grid;
        [SerializeField] private Tilemap _preview;

        public override void Initialize(Transform parent, Action onDespawn)
        {
            this.Log();
            base.Initialize(parent, onDespawn);
        }

        public override UniTask ShowAsync(CancellationToken token, ActorParam param, bool immediately = false)
        {
            this.Log();
            return base.ShowAsync(token, param, immediately);
        }

        public override UniTask HideAsync(CancellationToken token, bool immediately = false)
        {
            this.Log();
            return base.HideAsync(token, immediately);
        }
    }
}