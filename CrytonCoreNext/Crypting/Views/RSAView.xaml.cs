using CrytonCoreNext.Crypting.ViewModels;
using Wpf.Ui.Common.Interfaces;

namespace CrytonCoreNext.Crypting.Views
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
            DataContext = ViewModel;
        }
    }
}
