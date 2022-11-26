using CrytonCoreNext.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CrytonCoreNext.Interfaces
{
    public interface ICryptingService
    {
        Task<byte[]> RunCrypting(CryptFile file, IProgress<string> progress);

        void SetCurrentCrypting(ICrypting crypting);

        byte[] AddRecognitionBytes(CryptFile file);

        List<ICrypting> GetCryptors();

        ICrypting GetCurrentCrypting();

        int GetCurrentCryptingProgressCount();
    }
}
