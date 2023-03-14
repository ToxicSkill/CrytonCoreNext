using CrytonCoreNext.Crypting.Enums;
using CrytonCoreNext.Crypting.Interfaces;
using CrytonCoreNext.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace CrytonCoreNext.Crypting.Models
{
    public class CryptingRecognition : ICryptingRecognition
    {
        private const int MethodMaxSize = 14;

        private const int ExtensionMaxSize = 18;

        private readonly MD5 _MD5Hash;

        private static RecognitionValues _recognitionValues;

        private static readonly byte[] _defaultBytes = Array.Empty<byte>();

        private readonly Dictionary<string, (int size, byte[] value)> EXCFheader;

        public CryptingRecognition(RecognitionValues recognitionValues)
        {
            _recognitionValues = recognitionValues;
            _MD5Hash = MD5.Create();
            EXCFheader = new()
            {
                { nameof(_recognitionValues.Unique), new(_recognitionValues.Unique.ToByteArray().Length, _recognitionValues.Unique.ToByteArray()) },
                { nameof(_recognitionValues.Method), new(MethodMaxSize, _defaultBytes) },
                { nameof(_recognitionValues.Extension), new(ExtensionMaxSize, _defaultBytes) },
                { nameof(_recognitionValues.CheckSum), new(_MD5Hash.HashSize / 8, _defaultBytes) }
            };
        }

        public (bool succes, (EMethod method, string extension)) RecognizeBytes(byte[] bytes)
        {
            var maxSize = 0;
            foreach (var value in EXCFheader.Values)
            {
                maxSize += value.size;
            }

            if (maxSize > bytes.Length)
            {
                return new(false, new(EMethod.AES, string.Empty));
            }

            var recognizeByteArray = new byte[maxSize];
            Buffer.BlockCopy(bytes, 0, recognizeByteArray, 0, maxSize);
            var unique = new byte[EXCFheader[nameof(_recognitionValues.Unique)].size];
            Array.Copy(recognizeByteArray, 0, unique, 0, unique.Length);
            var method = new byte[EXCFheader[nameof(_recognitionValues.Method)].size];
            Array.Copy(recognizeByteArray, unique.Length, method, 0, method.Length);
            var extension = new byte[EXCFheader[nameof(_recognitionValues.Extension)].size];
            Array.Copy(recognizeByteArray, unique.Length + method.Length, extension, 0, extension.Length);
            var checkSum = new byte[EXCFheader[nameof(_recognitionValues.CheckSum)].size];
            Array.Copy(recognizeByteArray, unique.Length + method.Length + extension.Length, checkSum, 0, checkSum.Length);

            var checkArray = new byte[method.Length + extension.Length];
            Buffer.BlockCopy(method, 0, checkArray, 0, method.Length);
            Buffer.BlockCopy(extension, 0, checkArray, method.Length, extension.Length);
            var hashedArray = _MD5Hash.ComputeHash(checkArray);

            if (hashedArray.SequenceEqual(checkSum) && unique.SequenceEqual(EXCFheader[nameof(_recognitionValues.Unique)].value))
            {
                return new(true, new(StringToMethodEnum(GetStringFromByteArray(method)), GetStringFromByteArray(extension)));
            }

            return new(false, new(EMethod.AES, string.Empty));
        }

        public byte[] PrepareRerecognizableBytes(EMethod method, string extension)
        {
            var methodString = method.ToString();
            if (methodString.Length > EXCFheader[nameof(_recognitionValues.Method)].size)
            {
                methodString = methodString[..EXCFheader[nameof(_recognitionValues.Method)].size];
            }

            if (extension.Length > EXCFheader[nameof(_recognitionValues.Extension)].size)
            {
                extension = extension[..EXCFheader[nameof(_recognitionValues.Extension)].size];
            }

            var offset = 0;
            byte[] recognizableArray = new byte[EXCFheader.Sum(x => x.Value.size)];
            byte[] checkSum = new byte[EXCFheader[nameof(_recognitionValues.CheckSum)].size];

            EXCFheader[nameof(_recognitionValues.Method)] = new(EXCFheader[nameof(_recognitionValues.Method)].size, Encoding.ASCII.GetBytes(methodString));
            EXCFheader[nameof(_recognitionValues.Extension)] = new(EXCFheader[nameof(_recognitionValues.Extension)].size, Encoding.ASCII.GetBytes(extension));

            if (!SanityCheck())
            {
                return _defaultBytes;
            }

            offset = 0;
            var checkArray = new byte[EXCFheader[nameof(_recognitionValues.Method)].size + EXCFheader[nameof(_recognitionValues.Extension)].size];
            Buffer.BlockCopy(EXCFheader[nameof(_recognitionValues.Method)].value, 0, checkArray, 0, EXCFheader[nameof(_recognitionValues.Method)].value.Length);
            Buffer.BlockCopy(EXCFheader[nameof(_recognitionValues.Extension)].value, 0, checkArray, EXCFheader[nameof(_recognitionValues.Method)].size, EXCFheader[nameof(_recognitionValues.Extension)].value.Length);
            var hashedArray = _MD5Hash.ComputeHash(checkArray);
            EXCFheader[nameof(_recognitionValues.CheckSum)] = new(EXCFheader[nameof(_recognitionValues.CheckSum)].size, hashedArray);

            foreach (var item in EXCFheader)
            {
                Buffer.BlockCopy(item.Value.value, 0, recognizableArray, offset, item.Value.value.Length);
                offset += item.Value.size;
            }

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

        private bool SanityCheck()
        {
            foreach (var item in EXCFheader.Take(3))
            {
                if (item.Value.value.Length == 0)
                    return false;
            }
            return true;
        }
    }
}
