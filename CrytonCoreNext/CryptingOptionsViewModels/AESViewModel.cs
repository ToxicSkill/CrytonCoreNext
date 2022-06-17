using CrytonCoreNext.Abstract;
using CrytonCoreNext.Crypting;

namespace CrytonCoreNext.CryptingOptionsViewModels
{
    public class AESViewModel : ViewModelBase
    {
        private static AES Aes;

        public AESViewModel()
        {

        }

        public AESViewModel(AES aes)
        {
            Aes = aes;
        }
    }
}
