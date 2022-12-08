using CrytonCoreNext.Extensions;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;

namespace CrytonCoreNext.Crypting.Helpers
{
    public class AESHelper
    {
        private readonly AesCng _aes;

        public readonly PaddingMode _paddingMode;

        public List<int> LegalKeys = new();

        public List<int> LegalBlocks = new();

        public AESHelper(AesCng aes, PaddingMode paddingMode = PaddingMode.PKCS7)
        {
            _aes = aes;
            _paddingMode = paddingMode;
            _aes.Padding = _paddingMode;
            ParseLegalKeys();
        }

        private void ParseLegalKeys()
        {
            var legalKeys = _aes.LegalKeySizes[0];
            var legalBlocks = _aes.LegalBlockSizes[0];
            for (var key = legalKeys.MinSize; key <= legalKeys.MaxSize; key += legalKeys.SkipSize)
            {
                LegalKeys.Add(key);
            }

            for (var block = legalBlocks.MinSize; block <= legalBlocks.MaxSize; block += legalBlocks.SkipSize == 0 ? 1 : legalBlocks.SkipSize)
            {
                LegalBlocks.Add(block);
            }
        }

        public void GenerateNewKeys()
        {
            GenerateIV();
            GenerateKey();
        }

        public void GenerateKey()
        {
            _aes.GenerateKey();
        }

        public void GenerateIV()
        {
            _aes.GenerateIV();
        }

        public byte[] GetKey()
        {
            return _aes.Key;
        }

        public byte[] GetIV()
        {
            return _aes.IV;
        }

        public string GetKeyString()
        {
            return Convert.ToHexString(GetKey());
        }

        public string GetIVString()
        {
            return Convert.ToHexString(GetIV());
        }

        public int GetCurrentBlockSize()
        {
            return _aes.BlockSize;
        }

        public int GetCurrentKeySize()
        {
            return _aes.KeySize;
        }

        public void SetBlockSize(int blockSize)
        {
            if (LegalBlocks.Contains(blockSize))
            {
                _aes.BlockSize = blockSize;
            }
        }

        public void SetKeySize(int keySize)
        {
            if (LegalKeys.Contains(keySize))
            {
                _aes.KeySize = keySize;
            }
        }

        public bool ValidateKeys(string iv, string key)
        {
            if (iv.Equals(string.Empty) || key.Equals(string.Empty))
            {
                return false;
            }

            byte[]? byteKey;
            byte[]? byteIV;
            try
            {
                byteIV = iv.Str2Bytes();
                byteKey = key.Str2Bytes();
            }
            catch (Exception)
            {
                return false;
            }

            var keySize = byteKey.Length * 8;
            var ivSize = byteIV.Length * 8;

            if (IsBlockSizeValid(ivSize) && _aes.ValidKeySize(keySize))
            {
                _aes.KeySize = keySize;
                _aes.BlockSize = ivSize;
            }
            else
            {
                return false;
            }

            _aes.Key = byteKey;
            _aes.IV = byteIV;

            return true;
        }

        private bool IsBlockSizeValid(int blockSize)
        {
            return LegalBlocks.Contains(blockSize);
        }
    }
}
