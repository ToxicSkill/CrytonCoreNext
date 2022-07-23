using CrytonCoreNext.Abstract;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CrytonCoreNext.Interfaces
{
    public interface ICrypting
    {
        Task<byte[]> Encrypt(byte[] data, IProgress<string> progress);

        Task<byte[]> Decrypt(byte[] data, IProgress<string> progress);

        ViewModelBase GetViewModel();

        string GetName();

        bool ParseSettingsObjects(Dictionary<string, object> objects, bool encryption);
    }
}
