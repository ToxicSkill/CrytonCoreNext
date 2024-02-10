using CrytonCoreNext.Interfaces;
using CrytonCoreNext.ViewModels;
using CrytonCoreNext.Views;
using System;
using System.Windows;
using System.Windows.Input;
using Wpf.Ui;
using Wpf.Ui.Appearance;
using Wpf.Ui.Controls;



namespace CrytonCoreNext
{
    public partial class MainWindow : IWindow
    {
        public MainViewModel ViewModel
        {
            get;
        }

        public MainWindow(
                    MainViewModel viewModel,
                    INavigationService navigationService,
                    IServiceProvider serviceProvider,
                    ISnackbarService snackbarService,
                    IContentDialogService contentDialogService)
        {
            ViewModel = viewModel;
            DataContext = this;
            InitializeComponent();

            snackbarService.SetSnackbarPresenter(SnackbarPresenter);
            navigationService.SetNavigationControl(NavigationView);
            contentDialogService.SetContentPresenter(RootContentDialog);

            NavigationView.SetServiceProvider(serviceProvider);

            SystemThemeWatcher.Watch(this);
            LoadSettings();
        }

        private void LoadSettings()
        {
            LoadScreenModeFromSettings();
        }


        private void LoadScreenModeFromSettings()
        {
            Application.Current.MainWindow.WindowState =
                Properties.Settings.Default.FullscreenOnStart ?
                WindowState.Maximized :
                WindowState.Normal;
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
            if (actualState == WindowState.Maximized)
            {
                this.WindowState = WindowState.Normal;
            }
            else if (actualState == WindowState.Normal)
            {
                this.WindowState = WindowState.Maximized;
            }
        }

        private void NavigationView_SelectionChanged(NavigationView sender, RoutedEventArgs args)
        {
            if (sender is not Wpf.Ui.Controls.NavigationView navigationView)
            {
                return;
            }

            NavigationView.HeaderVisibility =
                navigationView.SelectedItem?.TargetPageType != typeof(Dashboard)
                    ? Visibility.Visible
                    : Visibility.Collapsed;
        }
    }
}
