using Domivium.Client.Core.Actors;
using UnityEngine;

namespace Domivium.Client.Core.Utility
{
    public static class Layer
    {
        public static readonly int Default = LayerMask.NameToLayer("Default");
        public static readonly int UI = LayerMask.NameToLayer("UI");
        public static readonly int Map = LayerMask.NameToLayer("Map");
        public static readonly int Effect = LayerMask.NameToLayer("Effect");
        public static readonly int Character = LayerMask.NameToLayer("Character");
        public static readonly int Tower = LayerMask.NameToLayer("Tower");

        public static readonly int CharacterMask = 1 << Character;
    }
}