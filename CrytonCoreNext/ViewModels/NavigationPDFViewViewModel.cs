using CrytonCoreNext.Abstract;
using CrytonCoreNext.Dictionaries;
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
            _pdfManagerViewModel.SendObject(Language.Post("Merge"));
            return _pdfManagerViewModel;
        }

        //private ViewModelBase NavigatePDFSplitPage()
        //{
        //    _pdfManagerViewModel.SendObject(_pdfSplitViewModel);
        //    _pdfManagerViewModel.SendObject(Language.Post("Split"));
        //    return _pdfManagerViewModel;
        //}

        //private ViewModelBase NavigatePDFConvertPage()
        //{
        //    _pdfManagerViewModel.SendObject(_pdfConvertViewModel);
        //    _pdfManagerViewModel.SendObject(Language.Post("Convert"));
        //    return _pdfManagerViewModel;
        //}
    }
}
