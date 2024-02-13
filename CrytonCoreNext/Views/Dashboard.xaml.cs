using CrytonCoreNext.BackgroundUI;
using CrytonCoreNext.Logo;
using CrytonCoreNext.ViewModels;
using Wpf.Ui.Controls;

namespace CrytonCoreNext.Views
{
    public partial class Dashboard : INavigableView<DashboardViewModel>
    {
        public DashboardViewModel ViewModel
        {
            get;
        }

        public Dashboard(DashboardViewModel viewModel)
        {
            ViewModel = viewModel;
            InitializeComponent();
            LogoFrame.Content = new Signet();
            Background.Content = new FluentWaves();
        }
    }
}
