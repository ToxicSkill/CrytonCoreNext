using CrytonCoreNext.Interfaces;
using CrytonCoreNext.Models;
using CrytonCoreNext.Static;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CrytonCoreNext.Services
{
    public class CryptingService : ICryptingService
    {
        private readonly ICryptingRecognition _cryptingRecognition;

        private readonly List<ICrypting> _cryptors;

        public ICrypting CurrentCrypting { get; private set; }

        public CryptingService(ICryptingRecognition cryptingRecognition, IEnumerable<ICrypting> cryptors)
        {
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

        public byte[] AddRecognitionBytes(CryptFile file)
        {

            if (file.Status.Equals(CryptingStatus.Status.Encrypted))
            {
                var recognitionBytes = _cryptingRecognition.PrepareRerecognizableBytes(file.Method, file.Extension);
                var newBytes = recognitionBytes.Concat(file.Bytes);
                if (recognitionBytes != null)
                {
                    if (recognitionBytes.Length > 0)
                    {
                        return newBytes.ToArray();
                    }
                }
            }
            return new byte[0];
        }

        public async Task<byte[]> RunCrypting(CryptFile file, IProgress<string> progress)
        {
            return file.Status.Equals(CryptingStatus.Status.Encrypted) ?
               await CurrentCrypting.Decrypt(file.Bytes, progress) :
               await CurrentCrypting.Encrypt(file.Bytes, progress);
        }

        public int GetCurrentCryptingProgressCount()
        {
            return CurrentCrypting.ProgressCount;
        }
    }
}
