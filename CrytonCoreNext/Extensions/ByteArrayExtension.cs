namespace CrytonCoreNext.Extensions
{
    public static class ByteArrayExtension
    {
        public static readonly string[] Sizes = { "B", "KB", "MB", "GB", "TB" };

        public static string GetSizeString(this byte[] bytes)
        {
            double len = bytes.Length;
            int order = 0;
            while (len >= 1024 && order < Sizes.Length - 1)
            {
                ++order;
                len /= 1024;
            }
            return string.Format("{0:0.## }{1}", len, Sizes[order]);
        }
    }
}