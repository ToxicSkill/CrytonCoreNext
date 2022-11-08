using CrytonCoreNext.Abstract;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CrytonCoreNext.Interfaces
{
    public interface ICrypting
    {
        public string Name { get; }

        public int ProgressCount { get; }

        Task<byte[]> Encrypt(byte[] data, IProgress<string> progress);

        Task<byte[]> Decrypt(byte[] data, IProgress<string> progress);

        ViewModelBase GetViewModel();

        string GetName();

        bool ParseSettingsObjects(Dictionary<string, object> objects, int dataLength, bool encryption);
    }
}
