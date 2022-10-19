using System;

/// <summary>
/// File status
/// </summary>
namespace CrytonCoreNext.Static
{
    public static class CryptingStatus
    {
        [Flags]
        public enum Status
        {
            Encrypted = 0,
            Decrypted = 1
        }
    }
}
        