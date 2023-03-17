using CrytonCoreNext.PDF.ViewModels;
using Wpf.Ui.Common.Interfaces;

namespace CrytonCoreNext.PDF.Views
{
    /// <summary>
    /// Interaction logic for PeopleView.xaml
    /// </summary>
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
        }
    }
}
