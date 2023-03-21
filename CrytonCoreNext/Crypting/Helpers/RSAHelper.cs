using System.Collections.Generic;
using System.Security.Cryptography;

namespace CrytonCoreNext.Crypting.Helpers
{
    public class RSAHelper
    {
        private int _keySize = 0;

        private readonly bool _useOAEP;

        private RSACryptoServiceProvider _rsaCryptoServiceProvider;

        public readonly int DefaultKeySize = 2048;

        public List<int> LegalKeys = new();

        public int KeyValueStep = 1;

        public int MaxFileSize = 0;


        public RSAHelper(bool useOAEP)
        {
            _useOAEP = useOAEP;
            _keySize = DefaultKeySize;
            _rsaCryptoServiceProvider = new(_keySize);
            MaxFileSize = GetMaxNumberOfBytes(2048);
            ParseLegalKeys();
        }

        public RSACryptoServiceProvider GetRSACryptoServiceProvider()
        {
            return _rsaCryptoServiceProvider;
        }

        public void SetKeySize(int keySize)
        {
            if (LegalKeys.Contains(keySize))
            {
                _keySize = keySize;
                MaxFileSize = GetMaxNumberOfBytes(keySize);
            }
        }

        public void GenerateKey()
        {
            _rsaCryptoServiceProvider = new(_keySize);
        }

        public string ToXmlString(bool includePrivate)
        {
            return _rsaCryptoServiceProvider.ToXmlString(includePrivate);
        }

        public void FromXmlString(string xmlString)
        {
            _rsaCryptoServiceProvider.FromXmlString(xmlString);
        }

        public int GetKeySize()
        {
            return _rsaCryptoServiceProvider.KeySize;
        }

        public byte[] GetPriveKey()
        {
            return _rsaCryptoServiceProvider.ExportRSAPrivateKey();
        }

        public byte[] GetPublicKey()
        {
            return _rsaCryptoServiceProvider.ExportRSAPublicKey();
        }

        public RSAParameters GetRSAParameters(bool includePrivate)
        {
            return _rsaCryptoServiceProvider.ExportParameters(includePrivate);
        }

        public int GetMaxNumberOfBytes()
        {
            return _useOAEP ? (_rsaCryptoServiceProvider.KeySize - 384) / 8 + 37 : (_rsaCryptoServiceProvider.KeySize - 384) / 8 + 7;
        }
        public int GetMaxNumberOfBytes(int size)
        {
            return _useOAEP ? (size - 384) / 8 + 37 : (size - 384) / 8 + 7;
        }

        public bool IsPrivateKeyAvailable()
        {
            return !_rsaCryptoServiceProvider.PublicOnly;
        }

        public bool IsKeyValid(int keySize)
        {
            return keySize % 128 == 0 &&
                keySize <= _rsaCryptoServiceProvider.LegalKeySizes[0].MaxSize &&
                keySize >= _rsaCryptoServiceProvider.LegalKeySizes[0].MinSize;
        }

        public int GetKeyValueStep()
        {
            return KeyValueStep;
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

        private void ParseLegalKeys()
        {
            var legalKeys = _rsaCryptoServiceProvider.LegalKeySizes[0];
            KeyValueStep = legalKeys.SkipSize * 16;
            for (var key = legalKeys.MinSize; key < legalKeys.MaxSize; key += KeyValueStep)
            {
                LegalKeys.Add(key);
            }
        }
    }
}
