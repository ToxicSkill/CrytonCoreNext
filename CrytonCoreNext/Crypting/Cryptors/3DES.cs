using CrytonCoreNext.Crypting.Enums;
using CrytonCoreNext.Crypting.Helpers;
using CrytonCoreNext.Crypting.Interfaces;
using CrytonCoreNext.Crypting.Models;
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

        private TripleDES _tripleDES;

        private static readonly byte[] Key = new byte[24];

        public EMethod Method => EMethod._3DES;

        public string DescriptionName => $"{Method} - Symmetric algorithm";

        public int ProgressCount => 1;

        public CryptingStatistics CryptingStatistics => new(4, 2, 3);

        public _3DES()
        {

            _tripleDES = TripleDES.Create();
            Key[0] = 37;
            Key[1] = 232;
            Key[2] = 166;
            Key[3] = 233;
            Key[4] = 45;
            Key[5] = 43;
            Key[6] = 88;
            Key[7] = 155;
            Key[8] = 191;
            Key[9] = 167;
            Key[10] = 103;
            Key[11] = 74;
            Key[12] = 192;
            Key[13] = 17;
            Key[14] = 208;
            Key[15] = 108;
            Key[16] = 1;
            Key[17] = 199;
            Key[18] = 105;
            Key[19] = 68;
            Key[20] = 52;
            Key[21] = 120;
            Key[22] = 209;
            Key[23] = 191;
        }

        public object GetHelper()
        {
            return null;
        }

        public async Task<byte[]> Encrypt(byte[] data, IProgress<string> progress)
        {
            _tripleDES.Key = Key;
            _tripleDES.Mode = _cipherMode;
            _tripleDES.Padding = _paddingMode;
            ICryptoTransform cTransform = _tripleDES.CreateEncryptor();
            return await Task.Run(() => Encrypt(data, cTransform, progress));
        }

        public async Task<byte[]> Decrypt(byte[] data, IProgress<string> progress)
        {
            _tripleDES.Key = Key;
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
