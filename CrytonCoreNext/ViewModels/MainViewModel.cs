using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CrytonCoreNext.Abstract;
using CrytonCoreNext.Interfaces;
using CrytonCoreNext.PDF.Views;
using CrytonCoreNext.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using Wpf.Ui.Common;
using Wpf.Ui.Controls;
using Wpf.Ui.Controls.Interfaces;
using Wpf.Ui.Controls.Navigation;

namespace CrytonCoreNext.ViewModels
{
    public partial class MainViewModel : ViewModelBase
    {
        private readonly ICustomPageService _pageService;

        private readonly List<string> _pdfTypeTags = new();

        private (INavigationItem view, string pageTag) _pdfNavigationviewPageTagLookup;

        private static readonly Type _pdfNavigationType = typeof(NavigationPDFView);

        [ObservableProperty]
        public ICollection<INavigationControl> menuItems;

        [ObservableProperty]
        public ICollection<INavigationControl> footerItems;

        public MainViewModel(ICustomPageService pageService)
        {
            _pageService = pageService;
            InitializeMenu();
            InitializePdfTypesTags();
            SetPdfNavigationView();
            _pageService.OnPageNavigate += HandlePageChange;
        }

        private void InitializeMenu()
        {
            MenuItems = new ObservableCollection<INavigationControl>()
            {
                new NavigationItem(){ Icon = SymbolRegular.Home20,  PageTag="home", Cache=true, Content="Home", PageType=typeof(Dashboard) },
                new NavigationSeparator(),
                new NavigationItem(){ Icon = SymbolRegular.Fingerprint48,  PageTag="crypting", Cache=true, Content="Crypting", PageType=typeof(CryptingView) },
                new NavigationItem(){ Icon = SymbolRegular.Layer20,  PageTag="pdf", Cache=true, Content="Pdf", PageType=typeof(NavigationPDFView) },
                new NavigationItem(){ Icon = SymbolRegular.Merge20,  PageTag="merge", Cache=true, Content="Merge", PageType=typeof(PdfMergeView) },
                new NavigationItem(){ Icon = SymbolRegular.ArrowSplit20,  PageTag="split", Cache=true, Content="Split", PageType=typeof(PdfSplitView) },
                new NavigationItem(){ Icon = SymbolRegular.ArrowSync20,  PageTag="convert", Cache=true, Content="Convert", PageType=typeof(PdfImageToPdfView) }
            };
            FooterItems = new ObservableCollection<INavigationControl>()
            {
                new NavigationItem(){ Icon = SymbolRegular.Settings20,  PageTag="settings", Cache=true, Content="Settings", PageType=typeof(SettingsView) }
            };
        }

        private void InitializePdfTypesTags()
        {
            _pdfTypeTags.Add(nameof(PdfMergeView));
            _pdfTypeTags.Add(nameof(PdfSplitView));
            _pdfTypeTags.Add(nameof(PdfImageToPdfView));
            _pdfTypeTags.Add(nameof(NavigationPDFView));
        }

        private void SetPdfNavigationView()
        {
            if (MenuItems.FirstOrDefault(item => (item as INavigationItem)?.PageType == typeof(NavigationPDFView)) is INavigationItem view)
            {
                _pdfNavigationviewPageTagLookup.view = view;
                _pdfNavigationviewPageTagLookup.pageTag = view.PageTag;
            }
        }

        private void HandlePageChange(object? sender, string pageName)
        {
            if (_pdfTypeTags.Contains(pageName))
            {
                if (pageName == _pdfNavigationType.Name)
                {
                    _pdfNavigationviewPageTagLookup.view.PageTag = _pdfNavigationviewPageTagLookup.pageTag;
                }
                else
                {
                    if (MenuItems.FirstOrDefault(item => (item as INavigationItem)?.PageType.Name == pageName) is INavigationItem newView)
                    {
                        _pdfNavigationviewPageTagLookup.view.PageTag = newView.PageTag;
                    }
                }
                OnPropertyChanged(nameof(MenuItems));
            }
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
