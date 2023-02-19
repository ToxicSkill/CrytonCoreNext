using CrytonCoreNext.Crypting.Interfaces;
using CrytonCoreNext.Crypting.Models;
using CrytonCoreNext.Models;
using CrytonCoreNext.Static;
using CrytonCoreNext.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static CrytonCoreNext.Static.CryptingStatus;

namespace CrytonCoreNext.Crypting.Services
{
    public class CryptingService : ICryptingService
    {
        private readonly ICryptingRecognition _cryptingRecognition;

        private readonly ICryptingReader _cryptingReader;

        private readonly List<ICryptingView<CryptingMethodViewModel>> _cryptingViews;

        private List<string> _methodsNames;

        public CryptingService(ICryptingRecognition cryptingRecognition,
            ICryptingReader cryptingReader,
            List<ICryptingView<CryptingMethodViewModel>> cryptingViews)
        {
            _cryptingReader = cryptingReader;
            _cryptingRecognition = cryptingRecognition;
            _cryptingViews = cryptingViews;
        }
        public List<ICryptingView<CryptingMethodViewModel>> GetCryptingViews()
        {
            return _cryptingViews;
        }

        public void AddRecognitionBytes(CryptFile file)
        {
            if (file.Status.Equals(Status.Encrypted))
            {
                var recognitionBytes = _cryptingRecognition.PrepareRerecognizableBytes(file.Method, file.Extension);
                var newBytes = recognitionBytes.Concat(file.Bytes);
                if (recognitionBytes != null)
                {
                    if (recognitionBytes.Length > 0)
                    {
                        file.Bytes = newBytes.ToArray();
                    }
                }
            }
        }

        public void ModifyFile(CryptFile file, byte[] bytes, Status status, string methodName)
        {
            file.Bytes = bytes;
            file.Status = status;
            file.Method = methodName ?? string.Empty;
            GC.Collect();
        }

        public async Task<byte[]> RunCrypting(ICryptingView<CryptingMethodViewModel> cryptingView, CryptFile file, IProgress<string> progress)
        {
            return file.Status.Equals(Status.Encrypted) ?
               await cryptingView.ViewModel.Crypting.Decrypt(file.Bytes, progress) :
               await cryptingView.ViewModel.Crypting.Encrypt(file.Bytes, progress);
        }

        public CryptFile ReadCryptFile(File file)
        {
            return _cryptingReader.ReadCryptFile(file, _cryptingRecognition.RecognizeBytes(file.Bytes));
        }

        public void RegisterFileChangedEvent(ref CryptingViewModel.HandleFileChanged? onFileChanged)
        {
            foreach (var view in _cryptingViews)
            {
                onFileChanged += view.ViewModel.HandleFileChanged;
            }
        }

        public bool IsCorrectMethod(CryptFile file, ICryptingView<CryptingMethodViewModel> cryptingView)
        {
            return file.Status == CryptingStatus.Status.Encrypted &&
                cryptingView.ViewModel.PageName != file.Method;
        }

        public CryptingStatus.Status GetOpositeStatus(CryptingStatus.Status currentStatus)
        {
            return currentStatus.Equals(CryptingStatus.Status.Decrypted) ?
                CryptingStatus.Status.Encrypted :
                CryptingStatus.Status.Decrypted;
        }
    }
}
