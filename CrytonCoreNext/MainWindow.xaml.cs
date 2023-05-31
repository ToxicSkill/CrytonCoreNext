using CrytonCoreNext.Interfaces;
using CrytonCoreNext.ViewModels;
using System;
using System.Windows.Controls;
using System.Windows.Input;
using Wpf.Ui.Appearance;
using Wpf.Ui.Controls.Interfaces;
using Wpf.Ui.Extensions;
using Wpf.Ui.Mvvm.Contracts;

namespace CrytonCoreNext
{
    public partial class MainWindow : INavigationWindow
    {
        public MainViewModel ViewModel
        {
            get;
        }

        public MainWindow(MainViewModel viewModel,
            INavigationService navigationService,
            ICustomPageService pageService,
            ISnackbarService snackbarService)
        {
            Watcher.Watch(this);
            ViewModel = viewModel;
            DataContext = ViewModel;
            InitializeComponent();

            SetPageService(pageService);
            navigationService.SetNavigationControl(RootNavigation);
            snackbarService.SetSnackbarControl(RootSnackbar);

            SetSystemTheme();
            ViewModel.ThemeStyleChanged += SetTheme;
        }

        public void SetTheme(BackgroundType value = BackgroundType.Mica)
        {
            this.WindowBackdropType = value;
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

        private void SymbolIcon_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                this.DragMove();
            }
        }

        private void SetSystemTheme()
        {
            Theme.Apply(Theme.GetSystemTheme().ToString().ToLower() == ThemeType.Dark.ToString().ToLower() ? ThemeType.Dark : ThemeType.Light, BackgroundType.Mica, true, true);
        }

        private void SymbolIcon_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var actualState = WindowState;
            if (actualState == System.Windows.WindowState.Maximized)
            {
                this.WindowState = System.Windows.WindowState.Normal;
            }
            else if (actualState == System.Windows.WindowState.Normal)
            {
                this.WindowState = System.Windows.WindowState.Maximized;
            }
        }
    }
}
