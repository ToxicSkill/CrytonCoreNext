using CommunityToolkit.Mvvm.ComponentModel;
using CrytonCoreNext.Drawers;
using CrytonCoreNext.Extensions;
using CrytonCoreNext.Models;
using OpenCvSharp;
using OpenCvSharp.WpfExtensions;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace CrytonCoreNext.AI.Models
{
    public partial class AIImage : SimpleImageItemContainer
    {
        private readonly ImageDrawer _drawer;

        private static readonly (double min, double max) _minMaxContrastRange = (0, 2);

        private static readonly (double min, double max) _minMaxExposureRange = (0, 2);

        private static readonly (double min, double max) _minMaxBrightnessRange = (-127, 127);

        private const int MaxSingleDimensionSize = 1024;

        public const double DefaultAutoColorValue = 0.5;

        public const double DefaultExposureValue = 1.0;

        public const double DefaultContrastValue = 1;

        public const int DefaultBrightnessValue = 0;

        public List<AIDetectionImage> DetectionImages { get; set; }

        public List<YoloPrediction> Predictions { get; private set; }

        public List<Path> Paths { get; set; }

        public Mat PipelineMat { get; set; }

        public Mat ResizedImage { get; set; }

        public Bitmap HistogramBitmap { get; set; }

        public bool RenderFinal { get; set; }

        public double TrueExposureValue { get; private set; } = DefaultExposureValue;

        public double TrueContrastValue { get; private set; } = DefaultContrastValue;

        public double TrueBrightnessValue { get; private set; } = DefaultBrightnessValue;

        [ObservableProperty]
        public double contrastValue = ConvertRange(_minMaxContrastRange.min, _minMaxContrastRange.max, 0, 100, DefaultContrastValue);

        [ObservableProperty]
        public double brightnessValue = ConvertRange(_minMaxBrightnessRange.min, _minMaxBrightnessRange.max, 0, 100, DefaultBrightnessValue);

        [ObservableProperty]
        public double exposureValue = ConvertRange(_minMaxExposureRange.min, _minMaxExposureRange.max, 0, 100, DefaultExposureValue);

        [ObservableProperty]
        public string detectionLabel;

        [ObservableProperty]
        public System.Drawing.Size constrains;

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
            Task.Run(() => { _ = _drawer.Post(this); });
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
            var constrains = new List<double>() { Image.Width, Image.Height };
            if (constrains.Any(x => x > MaxSingleDimensionSize))
            {
                var max = constrains.Max();
                var ratio = MaxSingleDimensionSize / max;
                var newHeight = Image.Height;
                var newWidth = Image.Width;
                if (Image.Width > Image.Height)
                {
                    newHeight *= ratio;
                    newWidth = MaxSingleDimensionSize;
                }
                else
                {
                    newWidth *= ratio;
                    newHeight = MaxSingleDimensionSize;
                }
                ResizedImage = Image.ToMat().EmptyClone();
                Cv2.Resize(Image.ToMat(), ResizedImage, new OpenCvSharp.Size(newWidth, newHeight));
            }
            Task.Run(UpdateImage);
        }

        partial void OnExposureValueChanging(double value)
        {
            TrueExposureValue = ConvertRange(0, 100, _minMaxExposureRange.min, _minMaxExposureRange.max, value);
            OnPropertyChanged(nameof(TrueExposureValue));
            UpdateImage();
        }

        partial void OnContrastValueChanged(double value)
        {
            TrueContrastValue = ConvertRange(0, 100, _minMaxContrastRange.min, _minMaxContrastRange.max, value);
            OnPropertyChanged(nameof(TrueContrastValue));
            UpdateImage();
        }

        partial void OnBrightnessValueChanged(double value)
        {
            TrueBrightnessValue = ConvertRange(0, 100, _minMaxBrightnessRange.min, _minMaxBrightnessRange.max, value);
            OnPropertyChanged(nameof(TrueBrightnessValue));
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

        private static double ConvertRange(double originalStart, double originalEnd,
                                        double newStart, double newEnd,
                                        double value)
        {
            double scale = (double)(newEnd - newStart) / (originalEnd - originalStart);
            return newStart + ((value - originalStart) * scale);
        }
    }
}
