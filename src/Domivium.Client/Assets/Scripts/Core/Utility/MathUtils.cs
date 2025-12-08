using UnityEngine;

namespace Domivium.Client.Core.Utility
{
    public static class MathUtils
    {
        public static Vector2 Random(float minR, float maxR)
        {
            var r = Mathf.Sqrt(UnityEngine.Random.Range(minR * minR, maxR * maxR));
            var angle = UnityEngine.Random.Range(0f, Mathf.PI * 2f);
            return new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * r;
        }
    }
}