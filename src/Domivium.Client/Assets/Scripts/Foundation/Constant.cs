// ReSharper disable CheckNamespace

using UnityEngine;

public static class Constant
{
    public const float Percent = 0.01f;
    public const float CanAttackRange = 0.95f;
    public const float AvoidCooldown = 1;

    public static Color MeleeColor = new(0.8980392f, 0.2235294f, 0.2078431f);
    public static Color RangedColor = new(0.117647f, 0.5333333f, 0.8980392f);
    public static Color TankColor = new(1f, 4392157f, 0.2627451f);
    public static Color SupportColor = new(0.2627451f, 0.6274511f, 0.2784314f);

    public static Color ActiveAttackButtonColor = new(1, 0.26f, 0.16f);
    public static Color IdleAttackButtonColor = new(1, 0.76f, 0.16f);
}