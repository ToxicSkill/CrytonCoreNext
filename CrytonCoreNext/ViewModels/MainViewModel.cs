using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CrytonCoreNext.Abstract;
using CrytonCoreNext.PDF.Views;
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
            InitializeMenu();
        }

        private void InitializeMenu()
        {
            MenuItems = new ObservableCollection<INavigationControl>()
            {
                new NavigationItem(){ Icon = SymbolRegular.Home20,  PageTag="home", Cache=true, Content="Home", PageType=typeof(Dashboard) },
                new NavigationSeparator(),
                new NavigationItem(){ Icon = SymbolRegular.Fingerprint48,  PageTag="crypting", Cache=true, Content="Crypting", PageType=typeof(CryptingView) },
                new NavigationItem(){ Icon = SymbolRegular.Layer20,  PageTag="pdf", Cache=true, Content="Pdf", PageType=typeof(PdfView) }
            };
            FooterItems = new ObservableCollection<INavigationControl>()
            {
                new NavigationItem(){ Icon = SymbolRegular.Settings20,  PageTag="settings", Cache=true, Content="Settings", PageType=typeof(SettingsView) }
            };
        }

        [RelayCommand]
        private void Exit()
        {
            App.Current.Shutdown();
        }

        [RelayCommand]
        private void Maximize()
        {
            Application.Current.MainWindow.WindowState = WindowState.Maximized;
        }

        [RelayCommand]
        private void Minimize()
        {
            Application.Current.MainWindow.WindowState = WindowState.Minimized;
        }

        [RelayCommand]
        private void Normal()
        {
            Application.Current.MainWindow.WindowState = WindowState.Normal;
        }
    }
}
