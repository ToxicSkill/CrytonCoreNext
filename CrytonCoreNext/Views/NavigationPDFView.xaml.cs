using CrytonCoreNext.ViewModels;
using Wpf.Ui.Common.Interfaces;

namespace CrytonCoreNext.Views
{
    /// <summary>
    /// Interaction logic for PeopleView.xaml
    /// </summary>
    public partial class NavigationPDFView : INavigableView<NavigationPDFViewViewModel>
    {
        public NavigationPDFViewViewModel ViewModel
        {
            get;
        }

        public NavigationPDFView(NavigationPDFViewViewModel viewModel)
        {
            ViewModel = viewModel;
            InitializeComponent();
            DataContext = ViewModel;
        }
    }
}
