namespace Utils.Extensions
{
    public static class StringExtensions
    {
        // ReSharper disable once InconsistentNaming
        public static int ComputeFNV1aHash(this string str)
        {
            uint hash = 2166136261;
            foreach (var c in str)
            {
                hash ^= c;
                hash *= 16777619;
            }

            return unchecked((int)hash);
        }
    }
}