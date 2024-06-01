using CrytonCoreNext.ViewModels.Camera;
using Wpf.Ui.Controls;

namespace CrytonCoreNext.Views.Camera
{
    public partial class CameraPreviewPage : INavigableView<CameraPreviewPageViewModel>
    {
        public CameraPreviewPageViewModel ViewModel
        {
            get;
        }

        public CameraPreviewPage(CameraPreviewPageViewModel viewModel)
        {
            ViewModel = viewModel;
            InitializeComponent();
            DataContext = ViewModel;
        }
    }
}
