using Domivium.Client.Core.Component;
using UnityEngine;

namespace Domivium.Client.Contents.Actors
{
    public class Tower : Pawn
    {
        [SerializeField] private LevelDisplay _levelDisplay;

        public void SetLevel(int level)
        {
            _levelDisplay.Set(level);
        }
    }
}