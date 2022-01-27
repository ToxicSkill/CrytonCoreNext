using CrytonCoreNext.Commands;
using CrytonCoreNext.Services;
using CrytonCoreNext.Stores;
using System.Windows.Input;

namespace CrytonCoreNext.ViewModels
{
    public class NavigationBarViewModel : ViewModelBase
    {
        public ICommand NavigateHomeCommand { get; }
        public ICommand NavigateCrytpingCommand { get; }
        public ICommand NavigatePasswordCommand { get; }
        public ICommand NavigatePdfManagerCommand { get; }
        public ICommand NavigateSettingsCommand { get; }
        

        public NavigationBarViewModel(INavigationService homeNavigationService,
            INavigationService cryptingNavigationService,
            INavigationService passwordNavigationService,
            INavigationService pdfManagerNavigationService,
            INavigationService settingsNavigationService)
        {
            NavigateHomeCommand = new NavigateCommand(homeNavigationService);
            NavigateCrytpingCommand = new NavigateCommand(cryptingNavigationService);
            NavigatePasswordCommand = new NavigateCommand(passwordNavigationService);
            NavigatePdfManagerCommand = new NavigateCommand(pdfManagerNavigationService);
            NavigateSettingsCommand = new NavigateCommand(settingsNavigationService);
        }


        public override void Dispose()
        {
            base.Dispose();
        }
    }
}
