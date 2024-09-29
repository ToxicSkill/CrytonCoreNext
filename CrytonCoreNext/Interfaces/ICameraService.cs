using CrytonCoreNext.Models;
using OpenCvSharp;
using System.Threading.Tasks;

namespace CrytonCoreNext.Interfaces
{
    public interface ICameraService
    {
        public Mat CurrentImage { get; }

        Task GetAllConnectedCameras();

        void UpdateCameraInfo(Camera camera);

        void SetCurrentCamera(Camera? camera);

        bool GrabCameraFrame();

        void SetFps(int fps);

        void SetBufferSize(int size);

        bool IsCameraOpen();

        bool IsCaptureDisposed();
    }
}
