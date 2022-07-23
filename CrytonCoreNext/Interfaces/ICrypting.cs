using CrytonCoreNext.Abstract;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CrytonCoreNext.Interfaces
{
    public interface ICrypting
    {
        Task<byte[]> Encrypt(byte[] data);

        Task<byte[]> Decrypt(byte[] data);

        ViewModelBase GetViewModel();

        string GetName();

        bool ParseSettingsObjects(Dictionary<string, object> objects, bool encryption);
    }
}
