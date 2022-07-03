using CrytonCoreNext.Interfaces;
using CrytonCoreNext.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace CrytonCoreNext.Crypting
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

        private bool SanityCheck()
        {
            foreach (var item in EXCFheader)
            {
                if (item.Value.value.Length == 0)
                    return false;
            }
            return true;
        }

        public byte[] PrepareRerecognizableBytes(string method, string extension)
        {
            if (method.Length > EXCFheader[nameof(_recognitionValues.Method)].size)
            {
                method = method[..EXCFheader[nameof(_recognitionValues.Method)].size];
            }
            if (extension.Length > EXCFheader[nameof(_recognitionValues.Extension)].size)
            {
                extension = extension[..EXCFheader[nameof(_recognitionValues.Extension)].size];
            }

            var offset = 0;
            byte[] recognizableArray = new byte[EXCFheader.Sum(x => x.Value.size)];
            byte[] checkSum = new byte[EXCFheader[nameof(_recognitionValues.CheckSum)].size];

            EXCFheader[nameof(_recognitionValues.Method)] = new(EXCFheader[nameof(_recognitionValues.Method)].size, Encoding.ASCII.GetBytes(method));
            EXCFheader[nameof(_recognitionValues.Extension)] = new(EXCFheader[nameof(_recognitionValues.Extension)].size, Encoding.ASCII.GetBytes(extension));

            Random rnd = new();
            rnd.NextBytes(checkSum);

            EXCFheader[nameof(_recognitionValues.CheckSum)] = new(EXCFheader[nameof(_recognitionValues.CheckSum)].size, checkSum);

            if (!SanityCheck())
            {
                return _defaultBytes;
            }

            offset = 0;
            var y = nameof(_recognitionValues.Method);
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
    }
}
