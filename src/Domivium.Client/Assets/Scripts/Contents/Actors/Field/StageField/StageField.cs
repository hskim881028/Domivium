using System.Collections.Generic;
using Domivium.Client.Contents.Components;
using UnityEngine;

namespace Domivium.Client.Contents.Actors
{
    public class StageField : Field
    {
        private readonly Dictionary<ushort, Vector2> _stageProps = new();

        public IReadOnlyDictionary<ushort, Vector2> StageProps => _stageProps;

        protected override void OnAwake()
        {
            base.OnAwake();
            var props = transform.GetComponentsInChildren<StageProp>();
            foreach (var prop in props)
            {
                _stageProps.Add(prop.LootId, prop.transform.position);
            }
        }
    }
}