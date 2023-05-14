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
            PdfPasswordBox.KeyDown += PdfPasswordBox_KeyDown;
        }

        private void PdfPasswordBox_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
            {
                UpdatePasswordBox();
                ViewModel.ConfirmPasswordCommand.Execute(null);
            }
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

        private void PdfPasswordBox_PreviewMouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var value = PdfPasswordBox.IsPasswordRevealed;
            LeftEyeIcon.Filled = value;
            RightEyeIcon.Filled = value;
        }

        private void TabControl_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {

        }
    }
}
