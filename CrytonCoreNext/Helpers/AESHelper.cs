using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;

namespace CrytonCoreNext.Helpers
{
    public class AESHelper
    {
        private readonly AesCng _aes;

        public readonly PaddingMode _paddingMode;

        public readonly string DefaultKeySize;

        public readonly string DefaultBlockSize;

        public List<string> LegalKeys = new();

        public List<string> LegalBlocks = new();

        public AESHelper(AesCng aes, PaddingMode paddingMode = PaddingMode.PKCS7)
        {
            _aes = aes;
            _paddingMode = paddingMode;
            ParseLegalKeys();
            DefaultKeySize = LegalKeys.First();
            DefaultBlockSize = LegalBlocks.First();
        }

        private void ParseLegalKeys()
        {
            var legalKeys = _aes.LegalKeySizes[0];
            var legalBlocks = _aes.LegalBlockSizes[0];
            for (var key = legalKeys.MinSize; key <= legalKeys.MaxSize; key += legalKeys.SkipSize)
            {
                LegalKeys.Add(key.ToString() ?? string.Empty);
            }

            for (var block = legalBlocks.MinSize; block <= legalBlocks.MaxSize; block += legalBlocks.SkipSize == 0 ? 1 : legalBlocks.SkipSize)
            {
                LegalBlocks.Add(block.ToString() ?? string.Empty);
            }
        }

        public bool IsIVValid(string iv)
        {
            return !iv.Equals(string.Empty);
        }

        public bool IsKeyValid(string key)
        {
            return !key.Equals(string.Empty);
        }
    }
}
