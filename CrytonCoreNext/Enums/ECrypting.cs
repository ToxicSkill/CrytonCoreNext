using System;

namespace CrytonCoreNext.Enums
{
    public static class ECrypting
    {
        public enum Methods : int
        {
            aes = 0
        }

        public static string EnumToString(Methods methods)
        {
            return methods switch
            {
                Methods.aes => nameof(Methods.aes),
                _ => throw new ArgumentOutOfRangeException(nameof(methods), methods, null),
            };
        }
    }
}
