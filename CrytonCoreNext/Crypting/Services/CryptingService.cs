using CrytonCoreNext.Crypting.Enums;
using CrytonCoreNext.Crypting.Interfaces;
using CrytonCoreNext.Crypting.Models;
using CrytonCoreNext.Models;
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

        private List<string> _methodsNames;

        public CryptingService(ICryptingRecognition cryptingRecognition,
            ICryptingReader cryptingReader)
        {
            _cryptingReader = cryptingReader;
            _cryptingRecognition = cryptingRecognition;
        }


        public void AddRecognitionBytes(CryptFile file)
        {
            if (file.Status.Equals(Status.Encrypted))
            {
                var recognitionBytes = _cryptingRecognition.PrepareRerecognizableBytes(file.Method, file.Extension, file.Keys);
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

        public void ModifyFile(CryptFile file, byte[] bytes, Status status, EMethod methodName)
        {
            file.Bytes = bytes;
            file.Status = status;
            file.Method = methodName;
            GC.Collect();
        }

        public async Task<byte[]> RunCrypting(ICrypting crypting, CryptFile file, IProgress<string> progress)
        {
            return file.Status.Equals(Status.Encrypted) ?
               await crypting.Decrypt(file.Bytes, progress) :
               await crypting.Encrypt(file.Bytes, progress);
        }

        public CryptFile ReadCryptFile(File file)
        {
            return _cryptingReader.ReadCryptFile(file, _cryptingRecognition.RecognizeBytes(file.Bytes));
        }

        public bool IsCorrectMethod(CryptFile file, ICryptingView<CryptingMethodViewModel> cryptingView)
        {
            if (file.Status == Status.Encrypted)
            {
                return cryptingView.ViewModel.Crypting.Method == file.Method;
            }
            else if (file.Status == Status.Decrypted)
            {
                return true;
            }
            return false;
        }

        public Status GetOpositeStatus(Status currentStatus)
        {
            return currentStatus.Equals(Status.Decrypted) ?
                Status.Encrypted :
                Status.Decrypted;
        }
    }
}
