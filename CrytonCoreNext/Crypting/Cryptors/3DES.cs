using CrytonCoreNext.Crypting.Enums;
using CrytonCoreNext.Crypting.Helpers;
using CrytonCoreNext.Crypting.Interfaces;
using CrytonCoreNext.Crypting.Models;
using Org.BouncyCastle.Crypto.Digests;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace CrytonCoreNext.Crypting.Cryptors
{
    public class _3DES : ICrypting
    {
        private static readonly PaddingMode _paddingMode = PaddingMode.PKCS7;

        private static readonly CipherMode _cipherMode = CipherMode.ECB;

        private static readonly string _password = "password";

        private readonly DESHelper _desHelper;

        private TripleDES _tripleDES;

        public EMethod Method => EMethod._3DES;

        public string DescriptionName => $"{Method} - Symmetric algorithm";

        public int ProgressCount => 1;

        public CryptingStatistics CryptingStatistics => new(4, 2, 3);

        public _3DES()
        {
            _desHelper = new DESHelper();
            _tripleDES = TripleDES.Create();
        }

        public object GetHelper()
        {
            return null;
        }

        public async Task<byte[]> Encrypt(byte[] data, IProgress<string> progress)
        {
            var bytesToTake = _tripleDES.LegalKeySizes[0].MaxSize / 8;
            var keyBytes = new byte[bytesToTake];
            var passwordBytes = SHA256.HashData(Encoding.ASCII.GetBytes(_desHelper.GetPassword()));
            Buffer.BlockCopy(passwordBytes, 0, keyBytes, 0, bytesToTake);
            _tripleDES.Key = keyBytes;
            _tripleDES.Mode = _cipherMode;
            _tripleDES.Padding = _paddingMode;
            ICryptoTransform cTransform = _tripleDES.CreateEncryptor();
            return await Task.Run(() => Encrypt(data, cTransform, progress));
        }

        public async Task<byte[]> Decrypt(byte[] data, IProgress<string> progress)
        {
            var bytesToTake = _tripleDES.LegalKeySizes[0].MaxSize / 8;
            var keyBytes = new byte[bytesToTake];
            var passwordBytes = SHA256.HashData(Encoding.ASCII.GetBytes(_password));
            Buffer.BlockCopy(passwordBytes, 0, keyBytes, 0, bytesToTake);
            _tripleDES.Key = keyBytes;
            _tripleDES.Mode = _cipherMode;
            _tripleDES.Padding = _paddingMode;
            ICryptoTransform cTransform = _tripleDES.CreateDecryptor();
            return await Task.Run(() => Decrypt(data, cTransform, progress));
        }

        public static byte[] Encrypt(byte[] data, ICryptoTransform cTransform, IProgress<string> progress)
        {
            return cTransform.TransformFinalBlock(data, 0,data.Length);
        }

        public static byte[] Decrypt(byte[] data, ICryptoTransform cTransform, IProgress<string> progress)
        {
            return cTransform.TransformFinalBlock( data, 0, data.Length);
        }
    }
}
