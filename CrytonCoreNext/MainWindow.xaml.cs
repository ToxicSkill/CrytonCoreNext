using CrytonCoreNext.Interfaces;
using CrytonCoreNext.ViewModels;
using System;
using System.Diagnostics;
using System.Windows.Controls;
using System.Windows.Input;
using Wpf.Ui.Appearance;
using Wpf.Ui.Controls.Interfaces;
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

            ViewModel.ThemeStyleChanged += SetTheme;
            LoadSettings();
        }

        public void SetTheme(BackgroundType value = BackgroundType.Mica)
        {
            try
            {
                this.WindowBackdropType = value;
            }
            catch (Exception e)
            {
                Trace.WriteLine(e.Message);
            }
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

        private void LoadSettings()
        {
            LoadThemeFromSettings();
            LoadScreenModeFromSettings();
        }

        private void LoadScreenModeFromSettings()
        {
            App.Current.MainWindow.WindowState = 
                Properties.Settings.Default.FullscreenOnStart ? 
                System.Windows.WindowState.Maximized :
                System.Windows.WindowState.Normal;
        }

        private void LoadThemeFromSettings()
        {
            if (Enum.TryParse(Properties.Settings.Default.Style, out BackgroundType backgroundTypeStyle))
            {
                SetTheme(backgroundTypeStyle);
            }
        }

        private void SymbolIcon_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                this.DragMove();
            }
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
