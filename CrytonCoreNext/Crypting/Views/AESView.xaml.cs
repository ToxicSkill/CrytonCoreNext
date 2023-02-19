using CrytonCoreNext.Crypting.Interfaces;
using CrytonCoreNext.Crypting.Models;

namespace CrytonCoreNext.Crypting.Views
{
    /// <summary>
    /// Interaction logic for AESView.xaml
    /// </summary>
    public partial class AESView : ICryptingView<CryptingMethodViewModel>
    {
        public CryptingMethodViewModel ViewModel { get; }

        public AESView(CryptingMethodViewModel viewModel)
        {
            ViewModel = viewModel;
            InitializeComponent();
            DataContext = ViewModel;
        }
    }
}
