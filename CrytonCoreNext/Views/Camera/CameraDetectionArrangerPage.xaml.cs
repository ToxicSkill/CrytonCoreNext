using CrytonCoreNext.ViewModels.Camera;
using Wpf.Ui.Controls;

namespace CrytonCoreNext.Views.Camera
{
    /// <summary>
    /// Interaction logic for CameraDetectionArrangerPage.xaml
    /// </summary>
    public partial class CameraDetectionArrangerPage : INavigableView<CameraDetectionArrangerPageViewModel>
    {
        public CameraDetectionArrangerPageViewModel ViewModel
        {
            get;
        }

        public CameraDetectionArrangerPage(CameraDetectionArrangerPageViewModel viewModel)
        {
            ViewModel = viewModel;
            InitializeComponent();
            DataContext = ViewModel;
        }
    }
}
