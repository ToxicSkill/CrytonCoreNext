using System.Collections.Generic;
using System.Security.Cryptography;

namespace CrytonCoreNext.Crypting.Helpers
{
    public class RSAHelper
    {
        private readonly bool _useOAEP;

        private readonly RSACryptoServiceProvider _rsa;

        public readonly int DefaultKeySize = 2048;

        public List<string> LegalKeys = new();

        public RSAHelper(bool useOAEP)
        {
            _useOAEP = useOAEP;
            _rsa = new(DefaultKeySize);
            ParseLegalKeys();
        }

        public RSAHelper(bool useOAEP, RSACryptoServiceProvider rsaCryptoServiceProvider)
        {
            _useOAEP = useOAEP;
            _rsa = rsaCryptoServiceProvider;
            ParseLegalKeys();
        }

        public int GetMaxNumberOfBytes(int keySize)
        {
            return _useOAEP ? (keySize - 384) / 8 + 37 : (keySize - 384) / 8 + 7;
        }

        public bool IsKeyEmpty(RSAParameters parameters)
        {
            return parameters.Modulus == null;
        }

        public bool IsKeyPrivate(RSAParameters parameters)
        {
            return parameters.D != null;
        }

        public bool IsKeyValid(int keySize)
        {
            return keySize % 128 == 0 &&
                keySize <= _rsa.LegalKeySizes[0].MaxSize &&
                keySize >= _rsa.LegalKeySizes[0].MinSize;
        }

        public RSAParameters GetOnlyPublicMembers(RSAParameters parameters)
        {
            var rsaParameters = new RSAParameters()
            {
                Modulus = parameters.Modulus,
                Exponent = parameters.Exponent
            };

            return rsaParameters;
        }

        public bool AreObjectsValid(RSAParameters parameters, int keySize)
        {
            return !IsKeyEmpty(parameters) && IsKeyValid(keySize);
        }

        private void ParseLegalKeys()
        {
            var legalKeys = _rsa.LegalKeySizes[0];
            for (var key = legalKeys.MinSize; key < legalKeys.MaxSize; key += legalKeys.SkipSize * 16)
            {
                LegalKeys.Add(key.ToString() ?? string.Empty);
            }
        }
    }
}
