using CrytonCoreNext.ViewModels.Camera;
using Wpf.Ui.Controls;

namespace CrytonCoreNext.Views.Camera
{
    /// <summary>
    /// Interaction logic for CamerasManagerPage.xaml
    /// </summary>
    public partial class CamerasManagerPage : INavigableView<CamerasManagerPageViewModel>
    {
        public CamerasManagerPageViewModel ViewModel
        {
            get;
        }

        public CamerasManagerPage(CamerasManagerPageViewModel viewModel)
        {
            ViewModel = viewModel;
            InitializeComponent();
            DataContext = ViewModel;
        }

        private void Page_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            ViewModel.ScanCamerasCommand.Execute(this);
        }
    }
}
