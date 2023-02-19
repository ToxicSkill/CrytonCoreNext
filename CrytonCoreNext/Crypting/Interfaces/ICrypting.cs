using CrytonCoreNext.Crypting.Enums;
using System;
using System.Threading.Tasks;

namespace CrytonCoreNext.Crypting.Interfaces
{
    public interface ICrypting
    {
        public EMethod Method { get; }

        public string DescriptionName { get; }

        public int ProgressCount { get; }

        object GetHelper();

        Task<byte[]> Encrypt(byte[] data, IProgress<string> progress);

        Task<byte[]> Decrypt(byte[] data, IProgress<string> progress);
    }
}
