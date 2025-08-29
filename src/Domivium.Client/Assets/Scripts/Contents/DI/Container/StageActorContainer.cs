using System.Collections.Generic;
using Domivium.Client.Core.Actors;
using UnityEngine;

namespace Domivium.Client.Contents.DI.Container
{
    [CreateAssetMenu(fileName = "StageActorContainer", menuName = "ScriptableObjects/StageActorContainer")]
    public class StageActorContainer : ScriptableObject
    {
        [SerializeField] private List<Actor> _actor;

        public List<Actor> Actor => _actor;
    }
}