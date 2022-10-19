using CrytonCoreNext.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CrytonCoreNext.Interfaces
{
    public interface ICryptingService
    {
        Task<byte[]> RunCrypting(File file, IProgress<string> progress);

        void SetCurrentCrypting(ICrypting crypting);

        List<ICrypting> GetCryptors();

        ICrypting GetCurrentCrypting();
    }
}
