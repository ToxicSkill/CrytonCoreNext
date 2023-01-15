﻿using CrytonCoreNext.Abstract;
using CrytonCoreNext.Dictionaries;
using System.Windows.Input;

namespace CrytonCoreNext.ViewModels
{
    public class NavigationPDFViewViewModel : ViewModelBase
    {
        private readonly ICommand _navigatePdfManagerCommand;

        private readonly InteractiveViewBase _pdfManagerViewModel;

        private readonly ViewModelBase _pdfMergeViewModel;

        private readonly ViewModelBase _pdfSplitViewModel;

        private readonly ViewModelBase _pdfImageToPdfViewModel;

        public ICommand NavigatePdfMergeCommand { get; }

        public ICommand NavigatePdfSplitCommand { get; }

        public ICommand NavigateImageToPdfCommand { get; }

        public NavigationPDFViewViewModel(InteractiveViewBase pdfManagerViewModel, ViewModelBase pdfMergeViewModel, ViewModelBase pdfSplitViewModel, ViewModelBase pdfImageToPdfViewModel)
        {
            _pdfMergeViewModel = pdfMergeViewModel;
            _pdfSplitViewModel = pdfSplitViewModel;
            _pdfManagerViewModel = pdfManagerViewModel;
            _pdfImageToPdfViewModel = pdfImageToPdfViewModel;
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

        private void NavigateImageToPdf()
        {
            _pdfManagerViewModel.SendObject(_pdfImageToPdfViewModel);
            _pdfManagerViewModel.SendObject(Language.Post("ImageToPdf"));
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
