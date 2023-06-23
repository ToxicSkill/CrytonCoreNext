using CrytonCoreNext.Crypting.Enums;
using CrytonCoreNext.Crypting.Models;
using CrytonCoreNext.Models;
using System;
using System.Threading.Tasks;
using static CrytonCoreNext.Static.CryptingStatus;

namespace CrytonCoreNext.Crypting.Interfaces
{
    public interface ICryptingService
    {
        Task<byte[]> RunCrypting(ICrypting crypting, CryptFile file, IProgress<string> progress);

        void ModifyFile(CryptFile file, byte[] bytes, Status status, EMethod method);

        void AddRecognitionBytes(CryptFile file);

        bool IsCorrectMethod(CryptFile file, ICryptingView<CryptingMethodViewModel> cryptingView);

        Status GetOpositeStatus(Status currentStatus);

        CryptFile ReadCryptFile(File file);
    }
}
