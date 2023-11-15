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
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Media.Imaging;
using static iText.Kernel.Pdf.Colorspace.PdfDeviceCs;

namespace CrytonCoreNext.AI.Models
{
    public partial class AIImage : SimpleImageItemContainer
    {
        private const double DefaultAutoColorValue = 0.5;

        private const double DefaultExposureValue = 0.5;

        private const double DefaultContrastValue = 1;

        private const int DefaultBrightnessValue = 0;

        public List<AIDetectionImage> DetectionImages { get; set; }

        public List<YoloPrediction> Predictions { get; private set; }

        [ObservableProperty]
        public System.Drawing.Size constrains;

        [ObservableProperty]
        public double autoColorValue = DefaultAutoColorValue;

        [ObservableProperty]
        public double contrastValue = DefaultContrastValue;

        [ObservableProperty]
        public double brightnessValue = DefaultBrightnessValue;

        [ObservableProperty]
        public double exposureValue = DefaultExposureValue;

        [ObservableProperty]
        public bool normalizeHistogram;

        [ObservableProperty]
        public WriteableBitmap histogram;

        [ObservableProperty]
        public WriteableBitmap detectionImage;

        [ObservableProperty]
        public WriteableBitmap adjusterImage;


        public AIImage(string path)
        {
            DetectionImages = new();
            Predictions = new ();
            Image = Cv2.ImRead(path).ToWriteableBitmap();
            Label = System.IO.Path.GetFileName(path);
            Constrains = new System.Drawing.Size((int)Image.Width, (int)Image.Height);
            DetectionImage = Image;
            AdjusterImage = Image;
            Histogram = HistogramDrawer.CalcualteHistogram(AdjusterImage);
        }

        private void UpdateImage()
        {
            AdjusterImage = Drawers.ImageDrawer.ApplyAll(Image.ToMat(), AutoColorValue, ContrastValue, BrightnessValue, ExposureValue, NormalizeHistogram).ToWriteableBitmap();
            Histogram = HistogramDrawer.CalcualteHistogram(AdjusterImage);
        }

        partial void OnContrastValueChanged(double value)
        {
            UpdateImage();
        }

        partial void OnBrightnessValueChanged(double value)
        {
            UpdateImage();
        }

        partial void OnAutoColorValueChanging(double value)
        {
            UpdateImage();
        }

        partial void OnNormalizeHistogramChanged(bool oldValue, bool newValue)
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
                DetectionImages.Add(
                    new(prediction)
                    {
                        Image = new Mat(mat, prediction.Rectangle.ToRect()).ToWriteableBitmap()
                    });
            }
        }

    }
}
