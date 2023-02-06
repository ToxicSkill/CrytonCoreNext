using CrytonCoreNext.ViewModels.Settings;
using Wpf.Ui.Common.Interfaces;

namespace CrytonCoreNext.Views.Settings
{
    public partial class AppearanceView : INavigableView<AppearanceViewModel>
    {
        public AppearanceViewModel ViewModel
        {
            get;
        }

        public AppearanceView(AppearanceViewModel viewModel)
        {
            ViewModel = viewModel;
            InitializeComponent();
            ViewModel = viewModel;
        }
    }
}
