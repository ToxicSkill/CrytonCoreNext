using CrytonCoreNext.Interfaces;
using CrytonCoreNext.Models;
using CrytonCoreNext.ViewModels.Camera;
using OpenCvSharp;
using OpenCvSharp.WpfExtensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace CrytonCoreNext.Services
{
    public enum ECameraType
    {
        USB,
        IP
    }

    public class CameraService : ICameraService
    {
        private readonly Mat _defaultWMat = new(new Size(1, 1), MatType.CV_8UC1);

        private readonly WriteableBitmap _defaultWriteableBitmap = new Mat(new Size(1, 1), MatType.CV_8UC1).ToWriteableBitmap();

        private readonly List<string> _connectionStrings;

        private int _currentCameraIndex;

        private readonly CameraContext _cameraContext;

        private VideoCapture _videoCapture;

        private Mat _image = new();

        public CameraService(CameraContext cameraContext)
        {
            _connectionStrings = GetConnectionStrings();
            _cameraContext = cameraContext;
            _cameraContext.AvailableCameras = [];
            _videoCapture = new();
        }

        public void SetCurrentCamera(Camera? camera)
        {
            if (camera == null)
            {
                return;
            }

            if (string.IsNullOrEmpty(camera.VideoCaptureConnectionString))
            {
                camera.VideoCapture = new VideoCapture(camera.VideoCaptureConnectionIndex);
            }
            else
            {
                camera.VideoCapture = new VideoCapture(camera.VideoCaptureConnectionString);
            }
            _videoCapture = camera.VideoCapture;
        }

        public void GrabCameraFrame()
        {
            if (_videoCapture.Grab())
            {
                _image = _videoCapture.RetrieveMat();
            }
        }

        public Mat GetLastCameraFrame()
        {
            if (!_image.Empty())
            {
                return _image;
            }
            return _defaultWMat;
        }

        public WriteableBitmap GetLastCameraFrameAsWriteableBitmap()
        {
            if (_image != null)
            {
                if (!_image.Empty())
                {
                    return _image.ToWriteableBitmap();
                }
            }
            return _defaultWriteableBitmap;
        }

        public void SetFps(int fps)
        {
            if (fps > 0)
            {
                _videoCapture.Set(VideoCaptureProperties.Fps, fps);
            }
        }

        public void SetBufferSize(int size)
        {
            if (size > 0)
            {
                _videoCapture.Set(VideoCaptureProperties.BufferSize, size);
            }
        }

        public bool IsCameraOpen()
        {
            return _videoCapture.IsOpened();
        }

        public bool IsCaptureDisposed()
        {
            return _videoCapture.IsDisposed;
        }

        public void UpdateCameraInfo(Camera camera)
        {
            camera.ImageSourceSize = $"{camera.VideoCapture.FrameWidth}x{camera.VideoCapture.FrameHeight}";
        }

        public async Task GetAllConnectedCameras()
        {
            _cameraContext.AvailableCameras = [];
            var cameraIndex = 0;
            var pnpCamerasCount = 0;
            var hasConnectionString = false;
            var camerasName = GetConnectedPnPCameras();
            pnpCamerasCount = camerasName.Count;
            camerasName.AddRange(GetConnectionStrings());
            foreach (var connectionString in camerasName)
            {
                var fps = 0;
                var name = "";
                if (await Task.Run(() =>
                {
                    VideoCapture? videoCapture = null;
                    if (cameraIndex == pnpCamerasCount)
                    {
                        videoCapture = new VideoCapture(connectionString);
                        hasConnectionString = true;
                    }
                    else
                    {
                        videoCapture = new VideoCapture(cameraIndex);
                        hasConnectionString = false;
                    }
                    if (videoCapture != null)
                    {
                        if (videoCapture.IsOpened())
                        {
                            name = videoCapture.GetBackendName();
                            fps = (int)videoCapture.Fps;
                            return true;
                        }
                    }
                    return false;
                }))
                {
                    if (hasConnectionString)
                    {
                        _cameraContext.AvailableCameras.Add(new Camera(name, connectionString, fps));
                    }
                    else
                    {
                        _cameraContext.AvailableCameras.Add(new Camera(name, cameraIndex, fps));
                    }
                }
                cameraIndex++;
            }
        }

        private static List<string> GetConnectionStrings()
        {
            var splittedConnectionStrings = new List<string>(Properties.Settings.Default.ConnectionStrings.Split("\n", StringSplitOptions.None));
            return splittedConnectionStrings.Where(x => x != string.Empty).ToList();
        }

        private static List<string> GetConnectedPnPCameras()
        {
            var cameraNames = new List<string>();
            using (ManagementObjectSearcher searcher = new("SELECT * FROM Win32_PnPEntity WHERE (PNPClass = 'Image' OR PNPClass = 'Camera')"))
            {
                foreach (var device in searcher.Get())
                {
                    var deviceName = device["Caption"].ToString() ?? string.Empty;
                    cameraNames.Add(deviceName);
                }
            }

            return cameraNames;
        }
    }
}
