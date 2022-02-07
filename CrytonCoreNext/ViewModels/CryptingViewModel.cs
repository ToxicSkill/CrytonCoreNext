using CrytonCoreNext.Abstract;
using CrytonCoreNext.Commands;

namespace CrytonCoreNext.ViewModels
{
    public class CryptingViewModel : ViewModelBase
    {
        public ViewModelBase InformationPopupViewModel { get; set; }

        public CryptingViewModel()
        {
            InformationPopupViewModel = new InformationPopupViewModel();
            ShowInformationBar(true);
        }
    }
}
