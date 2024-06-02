using CrytonCoreNext.Models;
using OpenCvSharp;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CrytonCoreNext.Interfaces
{
    public interface ICameraService
    {

        Task GetAllConnectedCameras();

        void UpdateCameraInfo(Camera camera);

        void SetCurrentCamera(Camera? camera);

        void GrabCameraFrame();

        Mat GetLastCameraFrame();

        void SetFps(int fps);

        void SetBufferSize(int size);

        bool IsCameraOpen();

        bool IsCaptureDisposed();
    }
}
