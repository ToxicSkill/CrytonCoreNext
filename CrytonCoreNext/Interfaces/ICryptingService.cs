using System.Collections.Generic;

namespace CrytonCoreNext.Interfaces
{
    public interface ICryptingService
    {
        void SetCurrentCrypting(ICrypting crypting);

        List<ICrypting> GetCryptors();

        ICrypting GetCurrentCrypting();
    }
}
