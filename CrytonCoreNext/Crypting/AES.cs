using CrytonCoreNext.Abstract;
using CrytonCoreNext.CryptingOptionsViewModels;
using CrytonCoreNext.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;

namespace CrytonCoreNext.Crypting
{
    public class AES : ICrypting
    {
        private static readonly string[] SettingsKeys = { "Key", "Block" };
        private const string Name = "AES";

        private readonly PaddingMode _paddingMode = PaddingMode.PKCS7;
        private readonly bool _status = true;
        private readonly AesCng _aes;

        public ViewModelBase ViewModel { get; init; }

        public AES()
        {
            _aes = new ();
            ViewModel = new AESViewModel(_aes);
        }

        public ViewModelBase GetViewModel() => ViewModel;
        

        public string GetName() =>  Name;
        

        public byte[] Encrypt(byte[] data)
        {
            ParseSettingsObjects(ViewModel.GetObjects());

            if (!_status)
                return default;

            _aes.Padding = _paddingMode;

            _aes.GenerateKey();
            _aes.GenerateIV();

            using (var encryptor = _aes.CreateEncryptor(_aes.Key, _aes.IV))
            {
                return PerformCryptography(data, encryptor);
            }
        }

        public byte[] Decrypt(byte[] data)
        {
            ParseSettingsObjects(ViewModel.GetObjects());

            if (!_status)
                return default;
            
            _aes.Padding = _paddingMode;

            //_aes.Key = _key;
            //_aes.IV = _iv;

            using (var decryptor = _aes.CreateDecryptor(_aes.Key, _aes.IV))
            {
                return PerformCryptography(data, decryptor);
            }
        }

        public void ParseSettingsObjects(Dictionary<string, object> objects)
        {
            _aes.KeySize = Convert.ToInt32(objects[SettingsKeys[0]]);
            _aes.BlockSize = Convert.ToInt32(objects[SettingsKeys[1]]);
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

        private byte[] GenerateRandomBytes(int size)
        {
            return RandomNumberGenerator.GetBytes(size);
        }
    }
}
