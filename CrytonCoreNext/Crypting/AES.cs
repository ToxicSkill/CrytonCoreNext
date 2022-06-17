using CrytonCoreNext.Abstract;
using CrytonCoreNext.Interfaces;
using System.IO;
using System.Security.Cryptography;

namespace CrytonCoreNext.Crypting
{
    public class AES : ICrypting
    {
        private const string Name = "AES";

        private readonly int _keySize = 128;
        private readonly int _blockSize = 128;
        private readonly byte[] _iv;
        private readonly byte[] _key;
        private readonly PaddingMode _paddingMode = PaddingMode.PKCS7;
        private readonly bool _status = true;

        public ViewModelBase ViewModel { get; set; }

        public AES(ViewModelBase viewModel)
        {
            ViewModel = viewModel;
            _iv = GenerateRandomBytes(_blockSize / 8);
            _key = GenerateRandomBytes(_keySize / 8);
            _status = SanityCheck();
        }

        public AES(int keySize, int blockSize, byte[] iv, byte[] key, PaddingMode paddingMode)
        {
            _keySize = keySize;
            _blockSize = blockSize;
            _iv = iv;
            _key = key;
            _paddingMode = paddingMode;
            _status = SanityCheck();
        }

        public AES(byte[] iv, byte[] key)
        {
            _iv = iv;
            _key = key;
            _status = SanityCheck();
        }

        public ViewModelBase GetViewModel() => ViewModel;
        

        public string GetName() =>  Name;
        

        private bool SanityCheck()
        {
            if ((_keySize % 2 != 0 && _keySize <= 0) ||
                (_blockSize % 2 != 0 && _blockSize <= 0) ||
                (_key.Length != _keySize / 8) ||
                (_iv.Length != _blockSize / 8))
                return false;
            return true;
        }

        private byte[] GenerateRandomBytes(int size)
        {
            return RandomNumberGenerator.GetBytes(size);
        }

        public byte[] Encrypt(byte[] data)
        {
            if (!_status)
                return default;

            using (var aes = Aes.Create())
            {
                aes.KeySize = _keySize;
                aes.BlockSize = _blockSize;
                aes.Padding = _paddingMode;

                aes.Key = _key;
                aes.IV = _iv;

                using (var encryptor = aes.CreateEncryptor(aes.Key, aes.IV))
                {
                    return PerformCryptography(data, encryptor);
                }
            }
        }

        public byte[] Decrypt(byte[] data)
        {
            if (!_status)
                return default;

            using (var aes = Aes.Create())
            {
                aes.KeySize = _keySize;
                aes.BlockSize = _blockSize;
                aes.Padding = _paddingMode;

                aes.Key = _key;
                aes.IV = _iv;

                using (var decryptor = aes.CreateDecryptor(aes.Key, aes.IV))
                {
                    return PerformCryptography(data, decryptor);
                }
            }
        }

        private byte[] PerformCryptography(byte[] data, ICryptoTransform cryptoTransform)
        {
            using (var ms = new MemoryStream())
            using (var cryptoStream = new CryptoStream(ms, cryptoTransform, CryptoStreamMode.Write))
            {
                cryptoStream.Write(data, 0, data.Length);
                cryptoStream.FlushFinalBlock();

                return ms.ToArray();
            }
        }
    }
}
