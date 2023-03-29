using CrytonCoreNext.BackgroundUI;
using CrytonCoreNext.ViewModels;
using Wpf.Ui.Common.Interfaces;

namespace CrytonCoreNext.Views
{
    public partial class PdfView : INavigableView<PdfViewModel>
    {
        public PdfViewModel ViewModel
        {
            get;
        }

        public PdfView(PdfViewModel viewModel)
        {
            ViewModel = viewModel;
            InitializeComponent();
            DataContext = ViewModel;
            Background.Content = new FluentWaves();
        }
    }
}
