using CrytonCoreNext.Crypting.Interfaces;
using CrytonCoreNext.Models;
using CrytonCoreNext.Static;
using System;

namespace CrytonCoreNext.Crypting.Models
{
    public class CryptFile : File
    {
        public CryptingStatus.Status Status { get; set; }

        public string Method { get; set; }

        public ICrypting Crypting { get; set; }

        public CryptFile(File file, CryptingStatus.Status status, string method, Guid guid) : base(file)
        {
            Status = status;
            Method = method;
            Guid = guid;
        }
    }
}
