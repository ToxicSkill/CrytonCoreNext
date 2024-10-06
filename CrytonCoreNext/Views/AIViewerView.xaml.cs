using CrytonCoreNext.AI.Models;
using CrytonCoreNext.BackgroundUI;
using CrytonCoreNext.Extensions;
using CrytonCoreNext.ViewModels;
using System;
using System.Windows;
using System.Windows.Controls;
using Wpf.Ui.Controls;

namespace CrytonCoreNext.Views
{
    public partial class AIViewerView : INavigableView<AIViewerViewModel>
    {
        private const int MagnifyValue = 15;

        private readonly ListViewItem _currentItem;

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
            if (sender is ListViewItem item)
            {
                if (item.Content is AIDetectionImage image)
                {
                    if (image != null && !Equals(_currentItem, item))
                    {
                        ViewModel.SelectedDetectionImage = image;
                    }
                }
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

        private void BeforeImage_PreviewMouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            var point = e.GetPosition(BeforeImage);
            var offset = 5;

            if (point.X < offset ||
                point.Y < offset ||
                point.X > BeforeImage.ActualWidth - offset ||
                point.Y > BeforeImage.ActualHeight - offset)
            {
                zoomGrid.Visibility = Visibility.Collapsed;
                return;
            }
            else
            {
                zoomGrid.Visibility = Visibility.Visible;
            }
            var scaleX = (double)zoomImage.ImageSource.Width / BeforeImage.ActualWidth;
            var scaleY = (double)zoomImage.ImageSource.Height / BeforeImage.ActualHeight;
            double marginX;
            double marginY;
            if (point.X < BeforeImage.ActualWidth / 2)
            {
                marginX = -BeforeImage.ActualWidth + point.X * 2;
            }
            else
            {
                marginX = point.X * 2 - BeforeImage.ActualWidth - (zoomGrid.ActualWidth + offset) * 2;
            }
            if (point.Y < BeforeImage.ActualHeight / 2)
            {
                marginY = -BeforeImage.ActualHeight + point.Y * 2;
            }
            else
            {
                marginY = point.Y * 2 - BeforeImage.ActualHeight - (zoomGrid.ActualHeight + offset) * 2;
            }
            marginX += zoomGrid.ActualWidth + offset;
            marginY += zoomGrid.ActualHeight + offset;
            zoomGrid.Margin = new Thickness()
            {
                Left = marginX,
                Top = marginY,
                Bottom = 0,
                Right = 0
            };
            point = new Point(point.X * scaleX, point.Y * scaleY);
            var region = MagnifyValue * Math.Max(BeforeImage.Source.Width, BeforeImage.Source.Height) / Math.Max(BeforeImage.ActualWidth, BeforeImage.ActualHeight);
            zoomImage.Viewbox = new Rect(
                point.X - region,
                point.Y - region,
                region * 2,
                region * 2);
        }
    }
}