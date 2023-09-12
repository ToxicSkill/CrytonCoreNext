using CrytonCoreNext.Crypting.Enums;
using CrytonCoreNext.Crypting.Interfaces;
using CrytonCoreNext.Enums;
using CrytonCoreNext.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection.Metadata;
using System.Security.Cryptography;
using System.Text;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace CrytonCoreNext.Crypting.Models
{

    public class CryptingRecognition : ICryptingRecognition
    {
        private readonly MD5 _MD5Hash;

        public CryptingRecognition()
        {
            _MD5Hash = MD5.Create();
        }

        public Recognition RecognizeBytes(byte[] bytes)
        {
            var recon = new Recognition();
            var maxSize = 0;
            foreach (var size in recon.SizeOf.Values)
            {
                maxSize += size;
            }

            if (maxSize > bytes.Length)
            {
                return recon;
            }

            var recognizeByteArray = new byte[maxSize];
            Buffer.BlockCopy(bytes, 0, recognizeByteArray, 0, maxSize);
            var method = new byte[recon.SizeOf[ERObject.Method]];
            Array.Copy(recognizeByteArray, 0, method, 0, recon.SizeOf[ERObject.Method]);
            var extension = new byte[recon.SizeOf[ERObject.Extension]];
            Array.Copy(recognizeByteArray, method.Length, extension, 0, recon.SizeOf[ERObject.Extension]);
            var keys = new byte[recon.SizeOf[ERObject.Keys]];
            Array.Copy(recognizeByteArray,  method.Length + extension.Length, keys, 0, recon.SizeOf[ERObject.Keys]);
            var checkSum = new byte[recon.SizeOf[ERObject.CheckSum]];
            Array.Copy(recognizeByteArray, method.Length + extension.Length + keys.Length, checkSum, 0, recon.SizeOf[ERObject.CheckSum]);

            var checkArray = new byte[
                recon.SizeOf[ERObject.Method] +
                recon.SizeOf[ERObject.Extension] +
                recon.SizeOf[ERObject.Keys]];

            Buffer.BlockCopy(method, 0, checkArray, 0, method.Length);
            Buffer.BlockCopy(extension, 0, checkArray, method.Length, extension.Length);
            Buffer.BlockCopy(keys, 0, checkArray, method.Length + extension.Length, keys.Length);
            var hashedArray = _MD5Hash.ComputeHash(checkArray);

            if (hashedArray.SequenceEqual(checkSum))
            {
                return new(EStatus.Success, StringToMethodEnum(GetStringFromByteArray(method)), GetStringFromByteArray(extension), GetStringFromByteArray(keys));
            }

            return recon;
        }

        public byte[] PrepareRerecognizableBytes(Recognition recon)
        {
            var methodString = recon.Method.ToString();
            if (methodString.Length > recon.SizeOf[ERObject.Method])
            {
                methodString = methodString[..recon.SizeOf[ERObject.Method]];
            }

            if (recon.Extension.Length > recon.SizeOf[ERObject.Extension])
            {
                recon.Extension = recon.Extension[..recon.SizeOf[ERObject.Extension]];
            }

            var offset = 0;
            byte[] recognizableArray = new byte[recon.SizeOf.Sum(x => x.Value)];
            byte[] checkSum = new byte[recon.SizeOf[ERObject.CheckSum]];

            var methodBytes = Encoding.ASCII.GetBytes(methodString);
            var extensionBytes = Encoding.ASCII.GetBytes(recon.Extension);
            var keysBytes = Encoding.ASCII.GetBytes(recon.Keys);

            offset = 0;
            var checkArray = new byte[
                recon.SizeOf[ERObject.Method] + 
                recon.SizeOf[ERObject.Extension] +
                recon.SizeOf[ERObject.Keys]];
            Buffer.BlockCopy(methodBytes, 0, checkArray, 0, methodBytes.Length);
            Buffer.BlockCopy(extensionBytes, 0, checkArray, recon.SizeOf[ERObject.Method], extensionBytes.Length);
            Buffer.BlockCopy(keysBytes, 0, checkArray, recon.SizeOf[ERObject.Method] + recon.SizeOf[ERObject.Extension], keysBytes.Length);
            var hashedArray = _MD5Hash.ComputeHash(checkArray);

            Buffer.BlockCopy(methodBytes, 0, recognizableArray, offset, methodBytes.Length);
            offset += recon.SizeOf[ERObject.Method];
            Buffer.BlockCopy(extensionBytes, 0, recognizableArray, offset, extensionBytes.Length);
            offset += recon.SizeOf[ERObject.Extension];
            Buffer.BlockCopy(keysBytes, 0, recognizableArray, offset, keysBytes.Length);
            offset += recon.SizeOf[ERObject.Keys];
            Buffer.BlockCopy(hashedArray, 0, recognizableArray, offset, hashedArray.Length);

            return recognizableArray;
        }

        private string GetStringFromByteArray(byte[] array)
        {
            var str = Encoding.Default.GetString(array);
            var indexToCut = str.IndexOf('\0');
            return str[..indexToCut];
        }

        private EMethod StringToMethodEnum(string methodString)
        {
            if (methodString.Equals(string.Empty))
            {
                return EMethod.AES;
            }
            if (Enum.TryParse(methodString, out EMethod enumMethod))
            {
                return enumMethod;
            }
            return EMethod.AES;
        }
    }
}
