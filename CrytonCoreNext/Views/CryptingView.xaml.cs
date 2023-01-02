using CrytonCoreNext.ViewModels;
using System.Windows.Controls;
using Wpf.Ui.Common.Interfaces;

namespace CrytonCoreNext.Views
{
    /// <summary>
    /// Interaction logic for PeopleView.xaml
    /// </summary>
    public partial class CryptingView : INavigableView<CryptingViewModel>
    {
        public CryptingViewModel ViewModel
        {
            get;
        }

        public CryptingView(CryptingViewModel viewModel)
        {
            ViewModel = viewModel;
            InitializeComponent();
        }
    }
}
