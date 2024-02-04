using CommunityToolkit.Mvvm.ComponentModel;
using CrytonCoreNext.Services;
using OpenCvSharp;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace CrytonCoreNext.Models
{
    public class CameraDetection(string label, string score)
    {
        public string Label { get; set; } = label;

        public string Score { get; set; } = score;

        public SolidColorBrush Color { get; set; } = StringToColor(label);

        private static SolidColorBrush StringToColor(string input)
        {
            var hash = input.GetHashCode();
            var red = (hash & 0xFF0000) >> 16;
            var green = (hash & 0x00FF00) >> 8;
            var blue = hash & 0x0000FF;

            return new(System.Windows.Media.Color.FromRgb((byte)red, (byte)green, (byte)blue));
        }
    }

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

        public VideoCapture VideoCapture { get; init; }

        public Camera(string name, VideoCapture videoCapture)
        {
            VideoCapture = videoCapture;
            Name = name;
            if (VideoCapture != null)
            {
                Fps = VideoCapture.Fps;
            }
        }
    }
}
