using CommunityToolkit.Mvvm.ComponentModel;
using CrytonCoreNext.Drawers;
using CrytonCoreNext.Extensions;
using CrytonCoreNext.Models;
using OpenCvSharp;
using OpenCvSharp.WpfExtensions;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace CrytonCoreNext.AI.Models
{
    public partial class AIImage : SimpleImageItemContainer
    {
        private readonly ImageDrawer _drawer;

        public const double DefaultAutoColorValue = 0.5;

        public const double DefaultExposureValue = 1.0;

        public const double DefaultContrastValue = 1;

        public const int DefaultBrightnessValue = 0;


        public CancellationTokenSource CancellationTokenSource { get; set; } = new();

        public List<AIDetectionImage> DetectionImages { get; set; }

        public List<YoloPrediction> Predictions { get; private set; }

        public List<Path> Paths { get; set; }

        public Mat PipelineMat { get; set; }

        [ObservableProperty]
        public object grid;

        [ObservableProperty]
        public System.Drawing.Size constrains;

        [ObservableProperty]
        public double contrastValue = DefaultContrastValue;

        [ObservableProperty]
        public double brightnessValue = DefaultBrightnessValue;

        [ObservableProperty]
        public double exposureValue = DefaultExposureValue;

        [ObservableProperty]
        public bool normalizeRGBHistogram;

        [ObservableProperty]
        public bool normalizeLABHistogram;

        [ObservableProperty]
        public WriteableBitmap histogram;

        [ObservableProperty]
        public WriteableBitmap detectionImage;

        [ObservableProperty]
        public WriteableBitmap adjusterImage;


        public AIImage(string path, ImageDrawer drawer)
        {
            _drawer = drawer;
            DetectionImages = [];
            Predictions = [];
            Path = path;
            LoadImages();
        }

        public void UpdateImage()
        {
            Task.Run(() => { _ = _drawer.Post(this, CancellationTokenSource.Token); });
        }

        private void LoadImages()
        {
            using var image = Cv2.ImRead(Path, ImreadModes.Unchanged);
            if (image.Empty())
            {
                return;
            }
            Image = image.ToWriteableBitmap();
            Label = System.IO.Path.GetFileName(Path);
            Constrains = new System.Drawing.Size((int)Image.Width, (int)Image.Height);
            DetectionImage = Image;
            AdjusterImage = Image;
            Task.Run(UpdateImage);
        }

        partial void OnExposureValueChanging(double value)
        {
            UpdateImage();
        }

        partial void OnContrastValueChanged(double value)
        {
            UpdateImage();
        }

        partial void OnBrightnessValueChanged(double value)
        {
            UpdateImage();
        }

        partial void OnNormalizeLABHistogramChanged(bool oldValue, bool newValue)
        {
            UpdateImage();
        }

        partial void OnNormalizeRGBHistogramChanged(bool oldValue, bool newValue)
        {
            UpdateImage();
        }

        public void SetPredicitons(List<YoloPrediction> predictions)
        {
            foreach (var prediction in Predictions)
            {
                var random = new Random();
                prediction.Label.Color = Color.FromArgb(random.Next(0, 255), random.Next(0, 255), random.Next(0, 255));
            }
            Predictions = predictions;
            ExtractDetectionImagesFromBitmap();
        }

        private void ExtractDetectionImagesFromBitmap()
        {
            using var mat = Image.ToMat();
            foreach (var prediction in Predictions)
            {
                var rectangle = prediction.Rectangle.ToRect();
                var newWidth = Math.Clamp(rectangle.Width, 0, mat.Width - rectangle.X);
                var newHeight = Math.Clamp(rectangle.Height, 0, mat.Height - rectangle.Y);
                var newX = Math.Clamp(rectangle.X, 0, mat.Width);
                var newY = Math.Clamp(rectangle.Y, 0, mat.Height);
                var newRect = new Rect(newX, newY, newWidth, newHeight);
                prediction.Rectangle = new RectangleF(newRect.X, newRect.Y, newRect.Width, newRect.Height);
                DetectionImages.Add(
                    new(this, prediction)
                    {
                        Image = new Mat(mat, newRect).ToWriteableBitmap()
                    });
            }
        }

    }
}
