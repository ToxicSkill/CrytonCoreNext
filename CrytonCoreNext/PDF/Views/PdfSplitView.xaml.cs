using CrytonCoreNext.PDF.ViewModels;
using Wpf.Ui.Common.Interfaces;

namespace CrytonCoreNext.PDF.Views
{
    /// <summary>
    /// Interaction logic for PeopleView.xaml
    /// </summary>
    public partial class PdfSplitView : INavigableView<PdfSplitViewModel>
    {
        public PdfSplitViewModel ViewModel
        {
            get;
        }

        public PdfSplitView(PdfSplitViewModel viewModel)
        {
            ViewModel = viewModel;
            InitializeComponent();
            DataContext = ViewModel;
        }
    }
}
