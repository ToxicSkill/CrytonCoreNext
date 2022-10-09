using System;

namespace CrytonCoreNext.Enums
{
    public static class ECryptingStatus
    {
        public enum CryptingStatus : int
        {
            Encrypted = 0,
            Decrypted
        }

        public static string EnumToString(CryptingStatus cryptingStatus)
        {
            return cryptingStatus switch
            {
                CryptingStatus.Encrypted => nameof(CryptingStatus.Encrypted),
                CryptingStatus.Decrypted => nameof(CryptingStatus.Decrypted),
                _ => throw new ArgumentOutOfRangeException(nameof(cryptingStatus), cryptingStatus, null),
            };
        }
    }
}
