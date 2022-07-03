using System.Linq;
using System.Security.Cryptography;

namespace CrytonCoreNext.Crypting
{
    public static class RandomCryptoGenerator
    {
        public static string GetCryptoRandomBytesString(int lenght)
        {
            using var csprng = RandomNumberGenerator.Create();
            var bytes = new byte[lenght];
            csprng.GetNonZeroBytes(bytes);
            return string.Join("", bytes.Select(b => b.ToString("X2")));
        }

        public static byte[] GetCryptoRandomBytesBytes(int lenght)
        {
            using var csprng = RandomNumberGenerator.Create();
            var bytes = new byte[lenght];
            csprng.GetNonZeroBytes(bytes);
            return bytes;
        }
    }
}
