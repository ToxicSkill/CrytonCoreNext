using CrytonCoreNext.Crypting.Enums;
using CrytonCoreNext.Crypting.Helpers;
using CrytonCoreNext.Crypting.Interfaces;
using CrytonCoreNext.Crypting.Models;
using CrytonCoreNext.Dictionaries;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace CrytonCoreNext.Crypting.Cryptors
{
    public class AES : ICrypting
    {
        private readonly PaddingMode _paddingMode = PaddingMode.PKCS7;

        private readonly AesCng _aes;

        private readonly AESHelper _aesHelper;

        public EMethod Method => EMethod.AES;

        public string DescriptionName => $"{Method} - Symmetric algorithm";

        public int ProgressCount => 1;

        public CryptingStatistics CryptingStatistics => new(3, 2, 3);

        public AES()
        {
            _aes = new();
            _aesHelper = new AESHelper(_aes, _paddingMode);
        }

        public object GetHelper()
        {
            return _aesHelper;
        }

        public async Task<byte[]> Encrypt(byte[] data, IProgress<string> progress)
        {
            using var encryptor = _aes.CreateEncryptor(_aes.Key, _aes.IV);
            return await Task.Run(() => PerformCryptography(data, encryptor, progress));
        }

        public async Task<byte[]> Decrypt(byte[] data, IProgress<string> progress)
        {
            using var decryptor = _aes.CreateDecryptor(_aes.Key, _aes.IV);
            return await Task.Run(() => PerformCryptography(data, decryptor, progress));
        }

        private byte[] PerformCryptography(byte[] data, ICryptoTransform cryptoTransform, IProgress<string> progress)
        {
            progress.Report(Language.Post("CollectingKeys"));
            using var ms = new MemoryStream();
            using var cryptoStream = new CryptoStream(ms, cryptoTransform, CryptoStreamMode.Write);
            cryptoStream.Write(data, 0, data.Length);
            try
            {
                cryptoStream.FlushFinalBlock();
            }
            catch (Exception)
            {
                return [];
            }
            return ms.ToArray();
        }
    }
}
