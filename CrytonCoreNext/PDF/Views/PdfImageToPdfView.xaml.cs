using CrytonCoreNext.PDF.ViewModels;
using Wpf.Ui.Common.Interfaces;

namespace CrytonCoreNext.PDF.Views
{
    /// <summary>
    /// Interaction logic for PeopleView.xaml
    /// </summary>
    public partial class PdfImageToPdfView : INavigableView<PdfImageToPdfViewModel>
    {
        public PdfImageToPdfViewModel ViewModel
        {
            get;
        }

        public PdfImageToPdfView(PdfImageToPdfViewModel viewModel)
        {
            ViewModel = viewModel;
            InitializeComponent();
            DataContext = ViewModel;
        }
    }
}
