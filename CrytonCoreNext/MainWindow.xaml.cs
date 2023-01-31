using CrytonCoreNext.ViewModels;
using System;
using System.Windows.Controls;
using Wpf.Ui.Controls.Interfaces;
using Wpf.Ui.Mvvm.Contracts;

namespace CrytonCoreNext
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : INavigationWindow
    {
        public MainViewModel ViewModel
        {
            get;
        }

        public MainWindow(MainViewModel viewModel, INavigationService navigationService, IPageService pageService, ISnackbarService snackbarService)
        {
            ViewModel = viewModel;
            DataContext = ViewModel;
            InitializeComponent();

            SetPageService(pageService);
            navigationService.SetNavigationControl(RootNavigation);
            snackbarService.SetSnackbarControl(RootSnackbar);
        }

        public Frame GetFrame()
            => RootFrame;

        public INavigation GetNavigation()
            => RootNavigation;

        public bool Navigate(Type pageType)
            => RootNavigation.Navigate(pageType);

        public void SetPageService(IPageService pageService)
            => RootNavigation.PageService = pageService;

        public void ShowWindow()
            => Show();

        public void CloseWindow()
            => Close();
    }
}
