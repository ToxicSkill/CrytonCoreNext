using CrytonCoreNext.Crypting.Interfaces;
using CrytonCoreNext.Crypting.ViewModels;

namespace CrytonCoreNext.Crypting.Views
{
    /// <summary>
    /// Interaction logic for AESView.xaml
    /// </summary>
    public partial class AESView : ICryptingView<AESViewModel>
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
