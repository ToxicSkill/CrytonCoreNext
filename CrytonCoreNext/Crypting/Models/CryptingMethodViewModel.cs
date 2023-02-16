using CrytonCoreNext.Abstract;

namespace CrytonCoreNext.Crypting.Models
{
    public partial class CryptingMethodViewModel : ViewModelBase
    {
        public delegate void HandleFileChanged();

        public event HandleFileChanged OnFileChanged;
    }
}
