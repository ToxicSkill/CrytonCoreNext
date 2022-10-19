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
        private readonly List<ICrypting> _cryptors;

        public ICrypting CurrentCrypting { get; private set; }

        public CryptingService(IEnumerable<ICrypting> cryptors)
        {
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

        public async Task<byte[]> RunCrypting(File file, IProgress<string> progress)
        {
            return file.Status.Equals(CryptingStatus.Status.Encrypted) ?
               await CurrentCrypting.Decrypt(file.Bytes, progress) :
               await CurrentCrypting.Encrypt(file.Bytes, progress);
        }
    }
}
