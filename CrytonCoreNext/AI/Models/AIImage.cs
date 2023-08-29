using CommunityToolkit.Mvvm.ComponentModel;
using CrytonCoreNext.Extensions;
using CrytonCoreNext.Models;
using OpenCvSharp;
using OpenCvSharp.WpfExtensions;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Media.Imaging;

namespace CrytonCoreNext.AI.Models
{
    public partial class AIImage : SimpleImageItemContainer
    {
        private const double DefaultAutoColorValue = 0.5;

        private const double DefaultContrastValue = 1;

        private const int DefaultBrightnessValue = 0;

        public List<AIDetectionImage> DetectionImages { get; set; }

        public List<YoloPrediction> Predictions { get; private set; }

        [ObservableProperty]
        public double autoColorValue = DefaultAutoColorValue;

        [ObservableProperty]
        public double contrastValue = DefaultContrastValue;

        [ObservableProperty]
        public double brightnessValue = DefaultBrightnessValue;

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
            DetectionImage = Image;
            AdjusterImage = Image;
        }

        partial void OnContrastValueChanged(double value)
        {
            AdjusterImage = Drawers.ImageDrawer.DrawScaleAbs(Image.ToMat(), ContrastValue, BrightnessValue).ToWriteableBitmap();
        }

        partial void OnBrightnessValueChanged(double value)
        {
            AdjusterImage = Drawers.ImageDrawer.DrawScaleAbs(Image.ToMat(), ContrastValue, BrightnessValue).ToWriteableBitmap();
        }

        partial void OnAutoColorValueChanging(double value)
        {
            AdjusterImage = Drawers.ImageDrawer.DrawAutoColor(Image.ToMat(), AutoColorValue).ToWriteableBitmap();
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
