using CrytonCoreNext.ViewModels;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Wpf.Ui.Common;
using Wpf.Ui.Common.Interfaces;
using Wpf.Ui.Controls;

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
    }
}