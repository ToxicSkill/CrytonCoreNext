using CommunityToolkit.Mvvm.ComponentModel;
using CrytonCoreNext.Drawers;
using CrytonCoreNext.Extensions;
using CrytonCoreNext.Models;
using OpenCvSharp;
using OpenCvSharp.Internal.Vectors;
using OpenCvSharp.WpfExtensions;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Media.Imaging; 

namespace CrytonCoreNext.AI.Models
{
    public partial class AIImage : SimpleImageItemContainer
    {
        private readonly ImageDrawer _drawer;

        public const double DefaultAutoColorValue = 0.5;

        public const double DefaultExposureValue = 0.5;

        public const double DefaultContrastValue = 1;

        public const int DefaultBrightnessValue = 0;

        public List<AIDetectionImage> DetectionImages { get; set; }

        public List<YoloPrediction> Predictions { get; private set; }

        public Mat PipelineMat { get; set; }

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
            DetectionImages = new();
            Predictions = new ();
            Image = Cv2.ImRead(path).ToWriteableBitmap();
            Label = System.IO.Path.GetFileName(path);
            Constrains = new System.Drawing.Size((int)Image.Width, (int)Image.Height);
            DetectionImage = Image;
            AdjusterImage = Image;
            Task.Run(UpdateImage);
        }

        private async Task UpdateImage()
        {
            await _drawer.Post(this);
        }

        partial void  OnContrastValueChanged(double value)
        {
            Task.Run(UpdateImage);
        }

        partial void OnBrightnessValueChanged(double value)
        {
            Task.Run(UpdateImage);
        }

        partial void OnNormalizeLABHistogramChanged(bool oldValue, bool newValue)
        {
            Task.Run(UpdateImage);
        }

        partial void OnNormalizeRGBHistogramChanged(bool oldValue, bool newValue)
        {
            Task.Run(UpdateImage);
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
                DetectionImages.Add(
                    new(prediction)
                    {
                        Image = new Mat(mat, prediction.Rectangle.ToRect()).ToWriteableBitmap()
                    });
            }
        }

    }
}
