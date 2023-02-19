using CrytonCoreNext.Crypting.Interfaces;
using CrytonCoreNext.Crypting.Models;

namespace CrytonCoreNext.Crypting.Views
{
    /// <summary>
    /// Interaction logic for RSAView.xaml
    /// </summary>
    public partial class RSAView : ICryptingView<CryptingMethodViewModel>
    {
        public CryptingMethodViewModel ViewModel { get; }

        public RSAView(CryptingMethodViewModel viewModel)
        {
            ViewModel = viewModel;
            InitializeComponent();
            DataContext = ViewModel;
        }
    }
}
