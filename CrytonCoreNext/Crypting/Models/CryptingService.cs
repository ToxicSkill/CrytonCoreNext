using CrytonCoreNext.Crypting.Interfaces;
using CrytonCoreNext.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static CrytonCoreNext.Static.CryptingStatus;

namespace CrytonCoreNext.Crypting.Models
{
    public class CryptingService : ICryptingService
    {
        private readonly ICryptingRecognition _cryptingRecognition;

        private readonly ICryptingReader _cryptingReader;

        private readonly List<ICrypting> _cryptors;

        public ICrypting CurrentCrypting { get; private set; }

        public CryptingService(ICryptingRecognition cryptingRecognition, ICryptingReader cryptingReader, IEnumerable<ICrypting> cryptors)
        {
            _cryptingReader = cryptingReader;
            _cryptingRecognition = cryptingRecognition;
            _cryptors = cryptors.ToList();
            SetCurrentCrypting(_cryptors.First());
        }

        public ICrypting GetCurrentCrypting()
        {
            return CurrentCrypting;
        }

        public List<ICrypting> GetCryptors()
        {
            return _cryptors.ToList();
        }

        public void SetCurrentCrypting(ICrypting crypting)
        {
            if (!_cryptors.Any())
            {
                return;
            }

            if (_cryptors.Contains(crypting))
            {
                CurrentCrypting = crypting;
            }
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
            Helpers.GCHelper.Collect();
        }

        public async Task<byte[]> RunCrypting(CryptFile file, IProgress<string> progress)
        {
            return file.Status.Equals(Status.Encrypted) ?
               await CurrentCrypting.Decrypt(file.Bytes, progress) :
               await CurrentCrypting.Encrypt(file.Bytes, progress);
        }

        public CryptFile ReadCryptFile(File file)
        {
            return _cryptingReader.ReadCryptFile(file, _cryptingRecognition.RecognizeBytes(file.Bytes));
        }

        public int GetCurrentCryptingProgressCount()
        {
            return CurrentCrypting.ProgressCount;
        }
    }
}
