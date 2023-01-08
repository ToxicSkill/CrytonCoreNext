using CrytonCoreNext.CryptingOptionsViewModels;
using Wpf.Ui.Common.Interfaces;

namespace CrytonCoreNext.CryptingOptionsViews
{
    /// <summary>
    /// Interaction logic for RSAView.xaml
    /// </summary>
    public partial class RSAView : INavigableView<RSAViewModel>
    {
        public RSAViewModel ViewModel { get; }

        public RSAView(RSAViewModel viewModel)
        {
            ViewModel = viewModel;
            InitializeComponent();
        }
    }
}
