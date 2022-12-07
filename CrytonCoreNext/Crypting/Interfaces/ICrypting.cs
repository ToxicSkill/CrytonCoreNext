using CrytonCoreNext.Abstract;
using System;
using System.Threading.Tasks;

namespace CrytonCoreNext.Crypting.Interfaces
{
    public interface ICrypting
    {
        public string Name { get; }

        public int ProgressCount { get; }

        Task<byte[]> Encrypt(byte[] data, IProgress<string> progress);

        Task<byte[]> Decrypt(byte[] data, IProgress<string> progress);

        ViewModelBase GetViewModel();

        string GetName();
    }
}
