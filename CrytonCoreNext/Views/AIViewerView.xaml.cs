using CrytonCoreNext.AI.Models;
using CrytonCoreNext.BackgroundUI;
using CrytonCoreNext.Extensions;
using CrytonCoreNext.ViewModels;
using System.Windows;
using System.Windows.Controls;
using Wpf.Ui.Controls;

namespace CrytonCoreNext.Views
{
    public partial class AIViewerView : INavigableView<AIViewerViewModel>
    {
        private ListViewItem _currentItem = null;

        public AIViewerViewModel ViewModel
        {
            get;
        }

        public AIViewerView(AIViewerViewModel viewModel)
        {
            ViewModel = viewModel;
            InitializeComponent();
            DataContext = ViewModel;
            Background.Content = new FluentWaves();
        }


        private void ListViewItem_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            var item = sender as ListViewItem;
            var detectionImage = (AIDetectionImage)item.Content;
            if (!Equals(_currentItem, item) && detectionImage != null)
            {
                ViewModel.SelectedDetectionImage = detectionImage;
            }
        }

        private void ListViewItem_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            ViewModel.SelectedDetectionImage = null;
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

        private void Button_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            ViewModel.ShowOriginal = true;
        }

        private void Button_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            ViewModel.ShowOriginal = false;
        }

        private void Slider_DragCompleted(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
        {
            ViewModel.SelectedImage.CancellationTokenSource.Cancel();
            ViewModel.SelectedImage.UpdateImage();
        }

        private void Slider_DragStarted(object sender, System.Windows.Controls.Primitives.DragStartedEventArgs e)
        {
            ViewModel.SelectedImage.CancellationTokenSource = new();
        }

        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {

        }
    }
}