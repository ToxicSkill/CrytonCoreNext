using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CrytonCoreNext.Interfaces;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace CrytonCoreNext.ViewModels.Camera
{
    public partial class CamerasManagerPageViewModel : ObservableObject
    {
        private readonly ICameraService _cameraService;

        [ObservableProperty]
        private bool _scanInProgress;

        [ObservableProperty]
        public ObservableCollection<Models.Camera> availableCameras;

        [ObservableProperty]
        public Models.Camera selectedCamera;

        public CamerasManagerPageViewModel(ICameraService cameraService)
        {
            _cameraService = cameraService;
        }

        partial void OnSelectedCameraChanged(Models.Camera value)
        {
            foreach (var camera in AvailableCameras)
            {
                camera.IsSelected = false;
            }
            if (value == null)
            {
                return;
            }
            value.IsSelected = true;
        }

        [RelayCommand]
        private async Task ScanCameras()
        {
            ScanInProgress = true;
            await _cameraService.GetAllConnectedCameras();
            var cameras = _cameraService.GetAllCameras();
            Models.Camera? camera = null;
            if (_cameraService.IsCameraOpen())
            {
                _cameraService.SetBufferSize(0);
                camera = _cameraService.GetCurrentCamera();
            }
            await System.Windows.Application.Current.Dispatcher.BeginInvoke(() =>
            {
                AvailableCameras = new(cameras);
                if (camera != null)
                {
                    SelectedCamera = camera;
                }
            });
            ScanInProgress = false;
        }
    }
}
