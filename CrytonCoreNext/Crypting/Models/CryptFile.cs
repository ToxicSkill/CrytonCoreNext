using CrytonCoreNext.Crypting.Enums;
using CrytonCoreNext.Crypting.Interfaces;
using CrytonCoreNext.Models;
using CrytonCoreNext.Static;
using System;

namespace CrytonCoreNext.Crypting.Models
{
    public class CryptFile : File
    {
        public CryptingStatus.Status Status { get; set; }

        public EMethod Method { get; set; }

        public ICrypting Crypting { get; set; }

        public Recognition Recognition { get; set; }

        public CryptFile(File file, CryptingStatus.Status status, Recognition recognition, Guid guid) : base(file)
        {
            Status = status;
            Method = recognition.Method;
            Guid = guid;
            Recognition = recognition;
        } 
    }
}
