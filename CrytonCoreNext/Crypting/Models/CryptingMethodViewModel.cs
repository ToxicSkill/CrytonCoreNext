using CrytonCoreNext.Abstract;

namespace CrytonCoreNext.Crypting.Models
{
    public partial class CryptingMethodViewModel : ViewModelBase
    {
        public CryptingMethodViewModel(string pageName) : base(pageName) { }

        public virtual void HandleFileChanged(CryptFile file) { }
    }
}
