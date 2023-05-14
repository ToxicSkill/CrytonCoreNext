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

        private void UpdatePasswordBox()
        {
            ViewModel.PdfPassword = PdfPasswordBox.Password;
        }

        private void Button_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            UpdatePasswordBox();
        }

        private void ListView_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            PdfPasswordBox.Password = string.Empty;
        }
    }
}
