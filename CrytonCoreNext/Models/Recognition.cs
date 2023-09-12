using CrytonCoreNext.Crypting.Enums;
using CrytonCoreNext.Enums;
using System;
using System.Collections.Generic;

namespace CrytonCoreNext.Models
{
    public struct Recognition
    {
        private const int MethodMaxSize = 14;

        private const int ExtensionMaxSize = 18;

        private const int KeysMaxSize = 1024;

        private const int CheckSumMaxSize = 16;

        public EMethod Method { get; set; }

        public string Extension { get; set; }

        public string Keys { get; set; }

        public EStatus Status { get; set; }

        public byte[] CheckSum { get; set; }

        public Dictionary<ERObject, int> SizeOf { get; private set; }

        public Recognition(EStatus status, EMethod method, string extension, string keys)
        {
            SizeOf = new();
            Status = status;
            Method = method;
            Extension = extension;
            Keys = keys;
            CreateDictionariy();
        }

        public Recognition()
        {
            SizeOf = new();
            Status = EStatus.Error;
            CreateDictionariy();
        }

        private void CreateDictionariy()
        {
            SizeOf = new ()
            {
                { ERObject.CheckSum, CheckSumMaxSize},
                { ERObject.Method, MethodMaxSize },
                { ERObject.Extension, ExtensionMaxSize },
                { ERObject.Keys, KeysMaxSize }
            };
        }
    }
}
