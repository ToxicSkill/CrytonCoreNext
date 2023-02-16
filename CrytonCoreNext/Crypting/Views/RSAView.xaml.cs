using CrytonCoreNext.Crypting.Interfaces;
using CrytonCoreNext.Crypting.ViewModels;

namespace CrytonCoreNext.Crypting.Views
{
    /// <summary>
    /// Interaction logic for RSAView.xaml
    /// </summary>
    public partial class RSAView : ICryptingView<RSAViewModel>
    {
        public RSAViewModel ViewModel { get; }

        public RSAView(RSAViewModel viewModel)
        {
            ViewModel = viewModel;
            InitializeComponent();
            DataContext = ViewModel;
        }
    }
}
