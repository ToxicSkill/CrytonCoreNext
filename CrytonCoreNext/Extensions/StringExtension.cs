using System;
using System.Linq;

namespace CrytonCoreNext.Extensions
{
    public static class StringExtension
    {
        public static byte[] Str2Bytes(this string hex)
        {
            return Enumerable.Range(0, hex.Length)
                             .Where(x => x % 2 == 0)
                             .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                             .ToArray();
        }
    }
}
