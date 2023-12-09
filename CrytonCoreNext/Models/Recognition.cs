using CrytonCoreNext.Crypting.Enums;
using CrytonCoreNext.Enums;

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
            Bytes = [];
        }
    }

    public class Recognition
    {
        public EMethod Method { get; set; }

        public string Extension { get; set; }

        public byte[] Keys { get; set; }

        public int KeysLenght { get; set; }

        public EStatus Status { get; set; } 

        public byte[] KeysCheckSum { get; set; }

        public Recognition(EStatus status, EMethod method, string extension, byte[] keys, int keysLenght, byte[] keysCheckArray)
        { 
            Status = status;
            Method = method;
            Extension = extension;
            Keys = keys;
            KeysLenght = keysLenght;
            KeysCheckSum = keysCheckArray;
        }

        public Recognition()
        { 
            Status = EStatus.Error;
        }

        public void SetKeys(byte[] keys)
        {
            Keys = keys;
        }
    }
}
