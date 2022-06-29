using CrytonCoreNext.Abstract;
using System.Security.Cryptography;

namespace CrytonCoreNext.CryptingOptionsViewModels
{
    public class RSAViewModel : ViewModelBase
    {
        public RSAViewModel(RSACng aes, string[] settingKeys, string pageName) : base(pageName)
        {
        }
    }
}