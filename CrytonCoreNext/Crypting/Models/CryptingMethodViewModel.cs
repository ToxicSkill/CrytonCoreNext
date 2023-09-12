using CrytonCoreNext.Abstract;
using CrytonCoreNext.Crypting.Interfaces;

namespace CrytonCoreNext.Crypting.Models
{
    public partial class CryptingMethodViewModel : ViewModelBase
    {
        public ICrypting Crypting { get; set; }

        public CryptingMethodViewModel(string pageName) : base(pageName) { }

        public virtual void HandleFileChanged(CryptFile file) { }

    }
}