using CrytonCoreNext.Crypting.ViewModels;
using Wpf.Ui.Common.Interfaces;

namespace CrytonCoreNext.Crypting.Views
{
    /// <summary>
    /// Interaction logic for AESView.xaml
    /// </summary>
    public partial class AESView : INavigableView<AESViewModel>
    {
        public AESViewModel ViewModel { get; }

        public AESView(AESViewModel viewModel)
        {
            ViewModel = viewModel;
            InitializeComponent();
            DataContext = ViewModel;
        }
    }
}
