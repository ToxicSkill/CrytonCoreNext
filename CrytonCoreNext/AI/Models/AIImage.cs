using CommunityToolkit.Mvvm.ComponentModel;
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
            CalcualteHistogram();
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

        private void CalcualteHistogram()
        {
            Cv2.Split(Image.ToMat(), out Mat[] planes);

            int[] histSize = { 256 };
            Rangef[] ranges = { new Rangef(0, 256) }; 
             
            Mat histB = new Mat();
            Mat histG = new Mat();
            Mat histR = new Mat();
            Cv2.CalcHist(new Mat[] { planes[0] }, new int[] { 0 }, null, histB, 1, histSize, ranges);
            Cv2.CalcHist(new Mat[] { planes[1] }, new int[] { 0 }, null, histG, 1, histSize, ranges);
            Cv2.CalcHist(new Mat[] { planes[2] }, new int[] { 0 }, null, histR, 1, histSize, ranges);


            var r = MatToArray(histR);
            var g = MatToArray(histG);
            var b = MatToArray(histB);
            var maxValue = (new int[] { r.maxValue, g.maxValue, b.maxValue }).Max();
            using var histogramMat = new Mat(new OpenCvSharp.Size(600, 200), MatType.CV_8UC4, new Scalar(0, 0, 0, 0));
            var rMat = DrawColorOnHistogram(r.values, Scalar.Red, maxValue);
            var gMat = DrawColorOnHistogram(g.values, Scalar.Green, maxValue);
            var bMat = DrawColorOnHistogram(b.values, Scalar.Blue, maxValue);
            Cv2.Add(histogramMat, bMat, histogramMat);
            Cv2.Add(histogramMat, gMat, histogramMat);
            Cv2.Add(histogramMat, rMat, histogramMat);
            //Cv2.Merge(new Mat[] { rMat, gMat, bMat, alphaMat }, histogramMat);             
            Histogram = histogramMat.ToWriteableBitmap();
            Cv2.ImWrite("C:\\Users\\gizmo\\OneDrive\\Obrazy\\Tests\\histr.png", histogramMat);
        }

        private Mat DrawColorOnHistogram(int[,] values, Scalar scalar, int maxValue)
        {
            var color = new Mat(new OpenCvSharp.Size(256, maxValue), MatType.CV_8UC4);
            for (var i = 0; i < 255; i++)
            {
                Cv2.Line(color, new OpenCvSharp.Point(i, maxValue), new OpenCvSharp.Point(i, maxValue - values[i, 0]), new Scalar(scalar.Val0, scalar.Val1, scalar.Val2, 0));
            }
            Cv2.Resize(color, color, new OpenCvSharp.Size(600, 200)); 
            return color;
        }

        static (int maxValue, int[,] values) MatToArray(Mat image)
        {
            var maxValue = 0;
            var pixelValues = new int[image.Rows, image.Cols];

            for (var row = 0; row < image.Rows; row++)
            {
                for (var col = 0; col < image.Cols; col++)
                {
                    var value = image.At<float>(row, col);
                    pixelValues[row, col] = (int)value;
                    if (value > maxValue)
                    {
                        maxValue = (int)value;
                    }
                }
            }

            return new (maxValue, pixelValues);
        }
    }
}
