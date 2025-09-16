namespace Domivium.Client.Editor
{
    public static class EditorUtils
    {
        public static int StableId(string name)
        {
            unchecked
            {
                var h = 2166136261;
                foreach (var t in name)
                {
                    h ^= t;
                    h *= 16777619;
                }
                return (int)(h & 0x3FFFFFFF);
            }
        }
    }
}