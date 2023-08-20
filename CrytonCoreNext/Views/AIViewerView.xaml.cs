using CrytonCoreNext.ViewModels;
using System.Windows.Controls;
using System.Windows;
using Wpf.Ui.Common.Interfaces;
using CrytonCoreNext.Extensions;

namespace CrytonCoreNext.Views
{
    public partial class AIViewerView : INavigableView<AIViewerViewModel>
    {
        public AIViewerViewModel ViewModel
        {
            get;
        }

        public AIViewerView(AIViewerViewModel viewModel)
        {
            ViewModel = viewModel;
            InitializeComponent();
            DataContext = ViewModel;
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