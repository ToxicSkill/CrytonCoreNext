using CommunityToolkit.Mvvm.ComponentModel;
using CrytonCoreNext.Services;
using OpenCvSharp;
using System.Windows.Media.Imaging;

namespace CrytonCoreNext.Models
{
    public partial class Camera : ObservableObject
    {
        [ObservableProperty]
        public ECameraType cameraType;

        [ObservableProperty]
        public string imageSourceSize;

        [ObservableProperty]
        public WriteableBitmap imageSource;

        [ObservableProperty]
        public string name;

        [ObservableProperty]
        public double fps;

        [ObservableProperty]
        public double currentFps;

        [ObservableProperty]
        public CameraDetection detectioZero;

        [ObservableProperty]
        public ObservableQueue<CameraDetection> cameraDetectionsQueue = [];

        public VideoCapture VideoCapture { get; set; }

        public string VideoCaptureConnectionString { get; set; }

        public int VideoCaptureConnectionIndex { get; set; }


        public Camera(string name, string connectionString, int fps)
        {
            VideoCaptureConnectionString = connectionString;
            Name = name;
            Fps = fps;
        }

        public Camera(string name, int connectionIndex, int fps)
        {
            VideoCaptureConnectionIndex = connectionIndex;
            Name = name;
            Fps = fps;
        }
    }
}
