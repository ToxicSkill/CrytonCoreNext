using CrytonCoreNext.Crypting.Enums;
using CrytonCoreNext.Enums;
using System;

namespace CrytonCoreNext.Models
{
    public struct RecognitionResult
    {
        public EStatus Status { get; init; }

        public byte[] Bytes { get; init; }

        public RecognitionResult(EStatus status, byte[] bytes)
        {
            Status = status;
            Bytes = bytes;
        }

        public RecognitionResult(EStatus status)
        {
            Status = status;
            Bytes = Array.Empty<byte>();
        }
    }

    public struct Recognition
    {

        public EMethod Method { get; set; }

        public string Extension { get; set; }

        public string Keys { get; set; }

        public EStatus Status { get; set; }

        public byte[] CheckSum { get; set; }

        public Recognition(EStatus status, EMethod method, string extension, string keys)
        { 
            Status = status;
            Method = method;
            Extension = extension;
            Keys = keys;
        }

        public Recognition()
        { 
            Status = EStatus.Error;
        }
    }
}
