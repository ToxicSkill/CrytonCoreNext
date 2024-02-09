using CrytonCoreNext.BackgroundUI;
using CrytonCoreNext.Extensions;
using CrytonCoreNext.ViewModels;
using System.Windows;
using System.Windows.Controls;
using Wpf.Ui.Controls;

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

        private void PdfPasswordBox_PreviewMouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var value = PdfPasswordBox.IsPasswordRevealed;
            var valueSecond = PdfPasswordBoxSecond.IsPasswordRevealed;
            LeftEyeIcon.Filled = value;
            RightEyeIcon.Filled = value;
            LeftEyeIconSecond.Filled = valueSecond;
            RightEyeIconSecond.Filled = valueSecond;
        }

        private void UpdatePasswordBox(bool isSecond = false)
        {
            ViewModel.SetPdfPassword(isSecond ? PdfPasswordBoxSecond.Password : PdfPasswordBox.Password);
        }

        private void PasswordSecond_Click(object sender, RoutedEventArgs e)
        {
            UpdatePasswordBox(true);
        }

        private void Password_Click(object sender, RoutedEventArgs e)
        {
            UpdatePasswordBox();
        }

        private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            LeftEyeIcon.Filled = true;
            RightEyeIcon.Filled = true;
            PdfPasswordBox.Password = string.Empty;
        }

        private void ListView_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {
            LeftEyeIconSecond.Filled = true;
            RightEyeIconSecond.Filled = true;
            if (ViewModel.SelectedPdfFile is not null)
            {
                PdfPasswordBoxSecond.Password = ViewModel.SelectedPdfFile.Password;
            }
        }

        private void Image_PreviewMouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
        {
            if (e.Delta > 0)
            {
                ViewModel.GoNextPageCommand.Execute(null);
            }
            else if (e.Delta < 0)
            {
                ViewModel.GoPreviousPageCommand.Execute(null);
            }
        }

        private void ListView_PreviewMouseDoubleClick_Merge(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            ViewModel.RemoveFileFromMergeListCommand.Execute(null);
        }

        private void ListView_PreviewMouseDoubleClick_Split(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            ViewModel.RemoveFileFromSplitListCommand.Execute(null);
        }


        private void Image_PreviewMouseWheel_Merge(object sender, System.Windows.Input.MouseWheelEventArgs e)
        {
            if (e.Delta > 0)
            {
                ViewModel.GoNextPagePdfToMergeIndexCommand.Execute(null);
            }
            else if (e.Delta < 0)
            {
                ViewModel.GoPreviousPagePdfToMergeIndexCommand.Execute(null);
            }
        }

        private void ListView_PreviewMouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
        {
            ScrollViewer scrollViewer = ((DependencyObject)sender).GetChildOfType<ScrollViewer>();
            if (e.Delta < 0)
            {
                scrollViewer.LineRight();
            }
            else
            {
                scrollViewer.LineLeft();
            }
            e.Handled = true;
        }

        private async void ListView_PreviewMouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            await ViewModel.DistributeSelectedPdfFile();
        }

        private void CardExpander_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (e.LeftButton == System.Windows.Input.MouseButtonState.Pressed && sender is FrameworkElement dep)
            {
                DragDrop.DoDragDrop(dep, new DataObject(DataFormats.Serializable, dep.DataContext), DragDropEffects.Move);
            }
        }

        private async void ListView_Drop(object sender, DragEventArgs e)
        {
            await ViewModel.DistributeSelectedPdfFile();
        }
    }
}
