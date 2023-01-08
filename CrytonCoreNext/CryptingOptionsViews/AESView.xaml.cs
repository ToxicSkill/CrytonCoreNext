using CrytonCoreNext.CryptingOptionsViewModels;
using Wpf.Ui.Common.Interfaces;

namespace CrytonCoreNext.CryptingOptionsViews
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
