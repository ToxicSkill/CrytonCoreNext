using CrytonCoreNext.Abstract;
using CrytonCoreNext.Commands;
using System.Windows.Input;

namespace CrytonCoreNext.ViewModels
{
    public class CryptingViewModel : ViewModelBase
    {
        public ViewModelBase InformationPopupViewModel { get; set; }

        public ICommand ShowInfoCommand { get; }
        public ICommand ShowErrorCommand { get; }

        public CryptingViewModel()
        {
            ShowInfoCommand = new Command(ShowInfo, true);
            ShowErrorCommand = new Command(ShowError, true);
        }

        private void ShowInfo()
        {
            InformationPopupViewModel = new InformationPopupViewModel("Info");
            ShowInformationBar(false);
            ShowInformationBar(true);
            OnPropertyChanged(nameof(InformationPopupViewModel));

        }

        private void ShowError()
        {
            InformationPopupViewModel = new InformationPopupViewModel("Error");
            ShowInformationBar(false);
            ShowInformationBar(true);
            OnPropertyChanged(nameof(InformationPopupViewModel));
        }
    }
}
