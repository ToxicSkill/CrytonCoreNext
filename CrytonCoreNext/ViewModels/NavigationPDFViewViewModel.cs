using CrytonCoreNext.Abstract;
using CrytonCoreNext.Commands;
using CrytonCoreNext.Dictionaries;
using CrytonCoreNext.Interfaces;
using CrytonCoreNext.Services;
using System.Windows.Input;

namespace CrytonCoreNext.ViewModels
{
    public class NavigationPDFViewViewModel : ViewModelBase
    {
        private readonly ICommand _navigatePdfManagerCommand;

        private readonly INavigate _navigator;

        private readonly InteractiveViewBase _pdfManagerViewModel;

        private readonly ViewModelBase _pdfMergeViewModel;

        private readonly ViewModelBase _pdfSplitViewModel;

        public ICommand NavigatePdfMergeCommand { get; }

        public ICommand NavigatePdfSplitCommand { get; }

        public NavigationPDFViewViewModel(INavigate navigator, InteractiveViewBase pdfManagerViewModel, ViewModelBase pdfMergeViewModel, ViewModelBase pdfSplitViewModel)
        {
            _navigator = navigator;
            _pdfMergeViewModel = pdfMergeViewModel;
            _pdfSplitViewModel = pdfSplitViewModel;
            _pdfManagerViewModel = pdfManagerViewModel;

            _navigatePdfManagerCommand = new NavigateService(_navigator, NavigatePDFManagerPage());

            NavigatePdfMergeCommand = new Command(NavigatePdfMerge, CanExecute);
            NavigatePdfSplitCommand = new Command(NavigatePdfSplit, CanExecute);
        }

        private void NavigatePdfSplit()
        {
            _pdfManagerViewModel.SendObject(_pdfSplitViewModel);
            _pdfManagerViewModel.SendObject(Language.Post("Split"));
            _navigatePdfManagerCommand.Execute(null);
        }

        private void NavigatePdfMerge()
        {
            _pdfManagerViewModel.SendObject(_pdfMergeViewModel);
            _pdfManagerViewModel.SendObject(Language.Post("Merge"));
            _navigatePdfManagerCommand.Execute(null);
        }

        private ViewModelBase NavigatePDFManagerPage()
        {
            return _pdfManagerViewModel;
        }

        //private ViewModelBase NavigatePDFConvertPage()
        //{
        //    _pdfManagerViewModel.SendObject(_pdfConvertViewModel);
        //    _pdfManagerViewModel.SendObject(Language.Post("Convert"));
        //    return _pdfManagerViewModel;
        //}
    }
}
