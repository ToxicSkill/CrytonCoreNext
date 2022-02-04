using CrytonCoreNext.Abstract;
using CrytonCoreNext.Interfaces;
using CrytonCoreNext.Services;
using System.Windows.Input;

namespace CrytonCoreNext.ViewModels
{
    public class NavigationViewModel : ViewModelBase
    {
        private readonly INavigate _navigator;

        public ICommand NavigateHomeCommand { get; }
        public ICommand NavigateCrytpingCommand { get; }
        
        public NavigationViewModel(INavigate navigator,
            ViewModelBase homeViewModel,
            ViewModelBase cryptingViewModel)
        {
            _navigator = navigator;

            NavigateHomeCommand = new NavigateService(_navigator, homeViewModel);
            NavigateCrytpingCommand = new NavigateService(_navigator, cryptingViewModel);
        }
    }
}
