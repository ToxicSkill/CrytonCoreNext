using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CrytonCoreNext.Interfaces;
using System.Threading.Tasks;

namespace CrytonCoreNext.ViewModels.Camera
{
    public partial class CamerasManagerPageViewModel : ObservableObject
    {
        private readonly ICameraService _cameraService;

        [ObservableProperty]
        private bool _scanInProgress;

        [ObservableProperty]
        private CameraContext _cameraContext;

        public CamerasManagerPageViewModel(CameraContext cameraContext, ICameraService cameraService)
        {
            _cameraService = cameraService;
            CameraContext = cameraContext;
        }

        [RelayCommand]
        private async Task ScanCameras()
        {
            ScanInProgress = true;
            await _cameraService.GetAllConnectedCameras();
            Models.Camera? camera = null;
            if (_cameraService.IsCameraOpen())
            {
                _cameraService.SetBufferSize(0);
            }
            await System.Windows.Application.Current.Dispatcher.BeginInvoke(() =>
            {
                if (camera != null)
                {
                    CameraContext.Camera = camera;
                }
            });
            ScanInProgress = false;
        }
    }
}
