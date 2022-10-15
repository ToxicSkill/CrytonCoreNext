using CrytonCoreNext.Interfaces;
using System.Collections.Generic;
using System.Linq;

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
    }
}
