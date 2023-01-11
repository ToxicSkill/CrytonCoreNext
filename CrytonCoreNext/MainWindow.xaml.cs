using CrytonCoreNext.ViewModels;
using System;
using System.Windows;
using System.Windows.Controls;
using Wpf.Ui.Appearance;
using Wpf.Ui.Common;
using Wpf.Ui.Controls.Interfaces;
using Wpf.Ui.Mvvm.Contracts;

namespace CrytonCoreNext
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : INavigationWindow
    {
        private bool _initialized = false;


        private readonly ISnackbarService _snackbarService;

        private readonly IThemeService _themeService;

        public MainViewModel ViewModel
        {
            get;
        }

        public MainWindow(MainViewModel viewModel, INavigationService navigationService, IPageService pageService, IThemeService themeService, ISnackbarService snackbarService)
        {
            ViewModel = viewModel;
            DataContext = this;

            _snackbarService = snackbarService;
            _themeService = themeService;
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

        private void NavigationButtonTheme_OnClick(object sender, RoutedEventArgs e)
        {
            _themeService.SetTheme(_themeService.GetTheme() == ThemeType.Dark ? ThemeType.Light : ThemeType.Dark);
        }

        private void TrayMenuItem_OnClick(object sender, RoutedEventArgs e)
        {
            if (sender is not MenuItem menuItem)
                return;
        }

        private void RootNavigation_OnNavigated(INavigation sender, RoutedNavigationEventArgs e)
        {
            RootFrame.Margin = new Thickness(
                left: 0,
                top: sender?.Current?.PageTag == "home" ? -69 : 0,
                right: 0,
                bottom: 0);
        }
    }
}
