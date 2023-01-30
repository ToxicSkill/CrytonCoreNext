using CommunityToolkit.Mvvm.ComponentModel;
using CrytonCoreNext.Abstract;
using CrytonCoreNext.PDF.Models;
using CrytonCoreNext.PDF.Views;
using System.Collections.ObjectModel;
using Wpf.Ui.Common.Interfaces;
using Wpf.Ui.Mvvm.Contracts;

namespace CrytonCoreNext.ViewModels
{
    public partial class NavigationPDFViewViewModel : ViewModelBase
    {
        private readonly INavigationService _navigationService;

        [ObservableProperty]
        public ObservableCollection<PDFPageItem> navigationItems;

        [ObservableProperty]
        public INavigableView<ViewModelBase> currentView;

        public NavigationPDFViewViewModel(INavigationService navigationService, PdfMergeView pdfMergeViewModel, PdfSplitView pdfSplitViewModel, PdfImageToPdfView pdfImageToPdfViewModel)
        {
            _navigationService = navigationService;
            navigationItems = new ObservableCollection<PDFPageItem>()
            {
                new PDFPageItem(Navigate)
                {
                    Icon = Wpf.Ui.Common.SymbolRegular.Merge20,
                    View = pdfMergeViewModel,
                    Type = typeof(PdfMergeView),
                    Description = "Merge pdf",
                    ShortDescription = "Lorem ipsum dolor sit amet, consectetur adipisicing elit, sed do eiusmod tempor incididunt ut labore et dolore magna"
                },
                new PDFPageItem(Navigate)
                {
                    Icon = Wpf.Ui.Common.SymbolRegular.ArrowSplit20,
                    View = pdfSplitViewModel,
                    Type = typeof(PdfSplitView),
                    Description = "Split pdf",
                    ShortDescription = "Lorem ipsum dolor sit amet, consectetur adipisicing elit, sed do eiusmod tempor incididunt ut labore et dolore magna"
                },
                new PDFPageItem(Navigate)
                {
                    Icon = Wpf.Ui.Common.SymbolRegular.ImageArrowBack20,
                    View = pdfImageToPdfViewModel,
                    Type = typeof(PdfImageToPdfView),
                    Description = "Convert pdf",
                    ShortDescription = "Lorem ipsum dolor sit amet, consectetur adipisicing elit, sed do eiusmod tempor incididunt ut labore et dolore magna"
                },
            };
            OnPropertyChanged(nameof(NavigationItems));
        }

        private void Navigate(INavigableView<ViewModelBase> view)
        {
            CurrentView = view;
            //_navigationService.Navigate(typeof(PdfMergeView));
        }
    }
}
