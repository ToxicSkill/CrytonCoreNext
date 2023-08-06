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
        private static readonly byte[] Key = new byte[24];

        public EMethod Method => EMethod._3DES;

        public string DescriptionName => $"{Method} - Symmetric algorithm";

        public int ProgressCount => 1;

        public CryptingStatistics CryptingStatistics => new(4, 2, 3);

        public _3DES()
        {
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
            return await Task.Run(() => Encrypt(data, false, progress));
        }

        public async Task<byte[]> Decrypt(byte[] data, IProgress<string> progress)
        {
            return await Task.Run(() => Decrypt(data, false, progress));
        }

        public static byte[] Encrypt(byte[] toEncrypt, bool useHashing, IProgress<string> progress)
        {

            byte[] toEncryptArray = toEncrypt;

            var tdes = TripleDES.Create();

            tdes.Key = Key;
            tdes.Mode = CipherMode.ECB;

            tdes.Padding = PaddingMode.PKCS7;

            ICryptoTransform cTransform = tdes.CreateEncryptor();
            byte[] resultArray =
              cTransform.TransformFinalBlock(toEncryptArray, 0,
              toEncryptArray.Length);
            tdes.Clear();
            return resultArray;
        }

        public static byte[] Decrypt(byte[] cipherString, bool useHash, IProgress<string> progress)
        {
            byte[] toDecryptArray = cipherString;

            var tdes = TripleDES.Create();
            tdes.Key = Key;

            tdes.Mode = CipherMode.ECB;
            tdes.Padding = PaddingMode.PKCS7;

            ICryptoTransform cTransform = tdes.CreateDecryptor();
            byte[] resultArray = cTransform.TransformFinalBlock(
                                 toDecryptArray, 0, toDecryptArray.Length);
            tdes.Clear();
            return resultArray;
        }
    }
}
