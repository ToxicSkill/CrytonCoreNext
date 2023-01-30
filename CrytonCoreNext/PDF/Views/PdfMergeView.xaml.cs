using CrytonCoreNext.PDF.ViewModels;
using Wpf.Ui.Common.Interfaces;

namespace CrytonCoreNext.PDF.Views
{
    /// <summary>
    /// Interaction logic for PeopleView.xaml
    /// </summary>
    public partial class PdfMergeView : INavigableView<PdfMergeViewModel>
    {
        public PdfMergeViewModel ViewModel
        {
            get;
        }

        public PdfMergeView(PdfMergeViewModel viewModel)
        {
            ViewModel = viewModel;
            InitializeComponent();
            DataContext = ViewModel;
        }
    }
}
