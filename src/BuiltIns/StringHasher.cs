namespace PlancakeSerializer.BuiltIns
{
    // DIY string hashing :D
    internal static class StringHasher
    {
        internal static long HashString(string str, int cmod = 1)
        {
            const int LongBits = sizeof(long) * 8;
            long r = 0;
            unchecked
            {
                for (int i = 0; i < str.Length; i++)
                {
                    char c = str[i];
                    r += (i * i) * c * cmod * (1L << (i % LongBits));
                }
            }
            return r;
        }
    }
}
