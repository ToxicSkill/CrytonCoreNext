﻿using CrytonCoreNext.Abstract;
using CrytonCoreNext.Interfaces;

namespace CrytonCoreNext.ViewModels
{
    public class MainViewModel : ViewModelBase, INavigate
    {
        public bool NavigationBarFolded { get; set; }

        public ViewModelBase ViewModel { get; set; }

        public NavigationViewModel NavigationView { get; set; }

        public NavigationPDFViewViewModel NavigationPDFViewView { get; set; }

        public MainViewModel(ViewModelBase homeViewModel,
            ViewModelBase cryptingViewModel,
            InteractiveViewBase pdfManagerViewModel,
            ViewModelBase pdfMergeViewModel)
        {
            ViewModel = homeViewModel;
            NavigationPDFViewView = new(this, pdfManagerViewModel, pdfMergeViewModel);
            NavigationView = new(this, homeViewModel, cryptingViewModel, NavigationPDFViewView);
            UpdateNavigationBar();
        }

        public void Navigate(ViewModelBase viewModel)
        {
            ViewModel = viewModel;
            OnPropertyChanged(nameof(ViewModel));
            UpdateNavigationBar();
        }

        private void UpdateNavigationBar()
        {
            NavigationBarFolded = ViewModel.PageName == "Home";
            OnPropertyChanged(nameof(NavigationBarFolded));
        }
    }
}
