using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CrytonCoreNext.Abstract;
using CrytonCoreNext.Views;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using Wpf.Ui.Controls;

namespace CrytonCoreNext.ViewModels
{
    public partial class MainViewModel : ViewModelBase
    {
        [ObservableProperty]
        public ICollection<object> menuItems;

        [ObservableProperty]
        public ICollection<object> footerItems;

        public MainViewModel()
        {
            MenuItems = [];
            FooterItems = [];
            InitializeMenu();
        }

        private void InitializeMenu()
        {
            MenuItems.Add(new NavigationViewItem("Home", SymbolRegular.Home20, typeof(Dashboard)));
            MenuItems.Add(new NavigationViewItemSeparator());
            MenuItems.Add(new NavigationViewItem("Crypting", SymbolRegular.Fingerprint48, typeof(CryptingView)));
            MenuItems.Add(new NavigationViewItem("Pdf", SymbolRegular.Layer20, typeof(PdfView)));
            MenuItems.Add(new NavigationViewItem("AI Viewer", SymbolRegular.Eye20, typeof(AIViewerView)));
            MenuItems.Add(new NavigationViewItem("AI Camera", SymbolRegular.CameraDome20, typeof(CameraView)));
            FooterItems.Add(new NavigationViewItemSeparator());
            FooterItems.Add(new NavigationViewItem("Settings", SymbolRegular.Settings20, typeof(SettingsView)));
            FooterItems.Add(new NavigationViewItem()
            {
                Content = "Quit",
                Icon = new SymbolIcon(SymbolRegular.ArrowExit20),
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
