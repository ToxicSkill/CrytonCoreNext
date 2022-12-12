using CrytonCoreNext.Abstract;
using CrytonCoreNext.Interfaces;
using CrytonCoreNext.Services;
using System.Windows.Input;

namespace CrytonCoreNext.ViewModels
{
    public class NavigationPDFViewViewModel : ViewModelBase
    {
        private readonly INavigate _navigator;

        public ICommand NavigateMergePageCommand { get; }

        public NavigationPDFViewViewModel(INavigate navigator, ViewModelBase PdfMergeViewModel)
        {
            _navigator = navigator;

            NavigateMergePageCommand = new NavigateService(_navigator, PdfMergeViewModel);
        }
    }
}
