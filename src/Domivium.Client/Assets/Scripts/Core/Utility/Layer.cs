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
        public static readonly int Monster = LayerMask.NameToLayer("Monster");
        public static readonly int Prop = LayerMask.NameToLayer("Prop");
        public static readonly int CharacterMask = 1 << Character;
        public static readonly int PropMask = 1 << Prop;
        public static readonly int MonsterMask = 1 << Monster;
        
        public static readonly int CharacterOrPropMask = CharacterMask | PropMask;
        public static readonly int MonsterOrPropMask = MonsterMask | PropMask;
    }
}