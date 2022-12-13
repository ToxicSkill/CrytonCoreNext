using CrytonCoreNext.Abstract;
using CrytonCoreNext.Interfaces;
using CrytonCoreNext.Services;
using System.Windows.Input;

namespace CrytonCoreNext.ViewModels
{
    public class NavigationPDFViewViewModel : ViewModelBase
    {
        private readonly INavigate _navigator;

        private readonly InteractiveViewBase _pdfManagerViewModel;

        private readonly ViewModelBase _pdfMergeViewModel;

        public ICommand NavigatePdfMergeCommand { get; }

        public NavigationPDFViewViewModel(INavigate navigator, InteractiveViewBase pdfManagerViewModel, ViewModelBase pdfMergeViewModel)
        {
            _navigator = navigator;
            _pdfMergeViewModel = pdfMergeViewModel;
            _pdfManagerViewModel = pdfManagerViewModel;
            NavigatePdfMergeCommand = new NavigateService(_navigator, NavigatePDFMergePage());
        }

        private ViewModelBase NavigatePDFMergePage()
        {
            _pdfManagerViewModel.SendObject(_pdfMergeViewModel);
            return _pdfManagerViewModel;
        }
    }
}
