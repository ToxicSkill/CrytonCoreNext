using CrytonCoreNext.Crypting.Models;
using CrytonCoreNext.Models;
using CrytonCoreNext.ViewModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using static CrytonCoreNext.Static.CryptingStatus;

namespace CrytonCoreNext.Crypting.Interfaces
{
    public interface ICryptingService
    {
        Task<byte[]> RunCrypting(CryptFile file, IProgress<string> progress);

        void SetCurrentCrypting(ICrypting crypting);

        void ModifyFile(CryptFile file, byte[] bytes, Status status, string methodName);

        List<ICrypting> GetCryptors();

        void AddRecognitionBytes(CryptFile file);

        ICrypting GetCurrentCrypting();

        int GetCurrentCryptingProgressCount();

        CryptFile ReadCryptFile(File file);

        void RegisterFileChangedEvent(ref CryptingViewModel.HandleFileChanged? onFileChanged);
    }
}
