using CrytonCoreNext.ViewModels;
using System.Windows.Controls;
using System.Windows;
using Wpf.Ui.Common.Interfaces;
using CrytonCoreNext.Extensions;
using CrytonCoreNext.AI.Models;
using CrytonCoreNext.BackgroundUI;

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
            ViewModel.OnTabControlChanged += UpdateCompareGridMaxWidth;
            compareGrid.SizeChanged += (s,e) => UpdateCompareGridMaxWidth();
        }

        private void UpdateCompareGridMaxWidth()
        {
            ViewModel.ImageCompareSliderValue = (int)(compareGrid.ActualWidth / 2);
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
    }
}