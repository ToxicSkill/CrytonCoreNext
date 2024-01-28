using CrytonCoreNext.Models;
using OpenCvSharp;
using System.Collections.Generic;

namespace CrytonCoreNext.Interfaces
{
    public interface ICameraService
    {
        string GetCurrentCameraName();

        List<Camera> GetAllCameras();

        void UpdateCameraInfo(Camera camera);

        Camera GetCurrentCamera();

        void SetCurrentCamera(Camera? camera);

        void GrabCameraFrame();

        Mat GetLastCameraFrame();

        void SetFps(int fps);

        void SetBufferSize(int size);

        bool IsCameraOpen();

        bool IsCaptureDisposed();
    }
}
