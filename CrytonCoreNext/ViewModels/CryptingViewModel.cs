using CrytonCoreNext.Abstract;
using CrytonCoreNext.Commands;
using System.Windows.Input;

namespace CrytonCoreNext.ViewModels
{
    public class CryptingViewModel : ViewModelBase
    {
        public ICommand ShowInfoCommand { get; }
        public ICommand ShowErrorCommand { get; }

        public CryptingViewModel()
        {
            ShowInfoCommand = new Command(ShowInfo, true);
            ShowErrorCommand = new Command(ShowError, true);
        }

        private void ShowInfo()
        {
            PostPopup("Info", new (0, 0, 5));
        }

        private void ShowError()
        {
            PostPopup("Error", new (0, 0, 5));
        }
    }
}
