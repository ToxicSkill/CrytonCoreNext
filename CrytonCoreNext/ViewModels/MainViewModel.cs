using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CrytonCoreNext.Abstract;
using CrytonCoreNext.Views;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using Wpf.Ui.Appearance;
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

        public delegate void OnThemeStyleChanged(BackgroundType value);

        public event OnThemeStyleChanged ThemeStyleChanged;

        public MainViewModel()
        {
            MenuItems = new ObservableCollection<INavigationControl>();
            FooterItems = new ObservableCollection<INavigationControl>();
            InitializeMenu();
        }

        public void InvokeThemeChanged(BackgroundType value)
        {
            ThemeStyleChanged?.Invoke(value);
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

            FooterItems.Add(new NavigationSeparator());
            FooterItems.Add(new NavigationItem()
            {
                Icon = SymbolRegular.Settings20,
                PageTag = "settings",
                Cache = true,
                Content = "Settings",
                PageType = typeof(SettingsView)
            });

            FooterItems.Add(new NavigationItem()
            {
                Icon = SymbolRegular.ArrowExit20,
                PageTag = "exit",
                Cache = false,
                Content = "Quit",
                Command = ExitCommand
            });
        }

        [RelayCommand]
        private static void Exit() => Application.Current.Shutdown();

        [RelayCommand]
        private static void Maximize() => Application.Current.MainWindow.WindowState = WindowState.Maximized;

        [RelayCommand]
        private static void Minimize() => Application.Current.MainWindow.WindowState = WindowState.Minimized;

        [RelayCommand]
        private static void Normal() => Application.Current.MainWindow.WindowState = WindowState.Normal;
    }
}
