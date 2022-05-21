using CrytonCoreNext.Abstract;
using CrytonCoreNext.Interfaces;

namespace CrytonCoreNext.ViewModels
{
    public class MainViewModel : ViewModelBase, INavigate
    {
        public ViewModelBase ViewModel { get; set; }

        public NavigationViewModel NavigationView { get; set; }

        public MainViewModel(ViewModelBase homeViewModel,
            ViewModelBase cryptingViewModel,
            ViewModelBase pdfManagerViewModel)
        {
            ViewModel = homeViewModel;
            NavigationView = new (this, homeViewModel, cryptingViewModel, pdfManagerViewModel);
        }

        public void Navigate(ViewModelBase viewModel)
        {
            ViewModel = viewModel;
            OnPropertyChanged(nameof(ViewModel));
        }
    }
}
