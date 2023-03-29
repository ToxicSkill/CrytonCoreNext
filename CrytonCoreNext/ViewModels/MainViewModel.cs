using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CrytonCoreNext.Abstract;
using CrytonCoreNext.Views;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using Wpf.Ui.Common;
using Wpf.Ui.Controls;
using Wpf.Ui.Controls.Interfaces;
using Wpf.Ui.Controls.Navigation;

namespace CrytonCoreNext.ViewModels
{
    public partial class MainViewModel : ViewModelBase
    {
        [ObservableProperty]
        public ICollection<INavigationControl> menuItems;

        [ObservableProperty]
        public ICollection<INavigationControl> footerItems;

        public MainViewModel()
        {
            MenuItems = new ObservableCollection<INavigationControl>();
            FooterItems = new ObservableCollection<INavigationControl>();
            InitializeMenu();
        }

        private void InitializeMenu()
        {
            MenuItems.Add(new NavigationItem()
            {
                Icon = SymbolRegular.Home20,
                PageTag = "home",
                Cache = true,
                Content = "Home",
                PageType = typeof(Dashboard)
            });
            MenuItems.Add(new NavigationSeparator());
            MenuItems.Add(new NavigationItem()
            {
                Icon = SymbolRegular.Fingerprint48,
                PageTag = "crypting",
                Cache = true,
                Content = "Crypting",
                PageType = typeof(CryptingView)
            });
            MenuItems.Add(new NavigationItem()
            {
                Icon = SymbolRegular.Layer20,
                PageTag = "pdf",
                Cache = true,
                Content = "Pdf",
                PageType = typeof(PdfView)
            });

            FooterItems.Add(new NavigationItem()
            {
                Icon = SymbolRegular.Settings20,
                PageTag = "settings",
                Cache = true,
                Content = "Settings",
                PageType = typeof(SettingsView)
            });
        }

        [RelayCommand]
        private static void Exit() => App.Current.Shutdown();

        [RelayCommand]
        private static void Maximize() => Application.Current.MainWindow.WindowState = WindowState.Maximized;

        [RelayCommand]
        private static void Minimize() => Application.Current.MainWindow.WindowState = WindowState.Minimized;

        [RelayCommand]
        private static void Normal() => Application.Current.MainWindow.WindowState = WindowState.Normal;
    }
}
