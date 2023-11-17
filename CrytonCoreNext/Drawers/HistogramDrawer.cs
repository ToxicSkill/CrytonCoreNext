using OpenCvSharp;
using OpenCvSharp.WpfExtensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace CrytonCoreNext.Drawers
{
    public static class HistogramDrawer
    {
        private const double ResizeRatio = 0.2;

        private const double HistogramRatio = 300/80;

        private const int MaxMatDimensionSize = 300;

        private static readonly Size HistogramSize = new (300, 80);

        public static WriteableBitmap CalcualteHistogram(WriteableBitmap image)
        {
            using var matForHistogram = image.ToMat();
            var maxDimension = new List<int>() { matForHistogram.Width, matForHistogram.Height }.Max();
            var newSize = new Size(matForHistogram.Width * ResizeRatio, matForHistogram.Height * ResizeRatio);
            if (newSize.Width > MaxMatDimensionSize || newSize.Height > MaxMatDimensionSize)
            {
                var doubles = new List<double>() { ((double)MaxMatDimensionSize / (double)newSize.Width), ((double)MaxMatDimensionSize / (double)newSize.Height) };
                var newRatio = doubles.Min();
                newSize = new Size(newSize.Width * newRatio, newSize.Height * newRatio);
            }
            Cv2.Resize(matForHistogram, matForHistogram, newSize);
            Cv2.Split(matForHistogram, out Mat[] planes);

            int[] histSize = { 256 };
            Rangef[] ranges = { new(0, 256) };

            Mat histB = new();
            Mat histG = new();
            Mat histR = new();
            Cv2.CalcHist(new Mat[] { planes[0] }, new int[] { 0 }, null, histB, 1, histSize, ranges);
            Cv2.CalcHist(new Mat[] { planes[1] }, new int[] { 0 }, null, histG, 1, histSize, ranges);
            Cv2.CalcHist(new Mat[] { planes[2] }, new int[] { 0 }, null, histR, 1, histSize, ranges);

            var r = MatToArray(histR);
            var g = MatToArray(histG);
            var b = MatToArray(histB);
            var maxValue = (new int[] { r.maxValue, g.maxValue, b.maxValue }).Max();
            var xRatio = HistogramSize.Width / maxValue; 
            using var histogramMat = new Mat(new Size(256, maxValue), MatType.CV_8UC4, new Scalar(0, 0, 0, 0));
            var rMat = DrawColorOnHistogram(r.values, Scalar.Red, maxValue);
            var gMat = DrawColorOnHistogram(g.values, Scalar.Green, maxValue);
            var bMat = DrawColorOnHistogram(b.values, Scalar.Blue, maxValue);
            Cv2.Add(histogramMat, bMat, histogramMat);
            Cv2.Add(histogramMat, gMat, histogramMat);
            Cv2.Add(histogramMat, rMat, histogramMat);
            Cv2.Resize(histogramMat, histogramMat, HistogramSize);
            Cv2.GaussianBlur(histogramMat, histogramMat, new Size(3, 3), 0);
            return histogramMat.ToWriteableBitmap();
        }

        private static Mat DrawColorOnHistogram(int[,] values, Scalar scalar, int maxValue)
        {
            var color = new Mat(new Size(256, maxValue), MatType.CV_8UC4, new Scalar(0, 0, 0, 0));
            for (var i = 0; i < 255; i++)
            {
                Cv2.Line(color, new Point(i, maxValue), new Point(i, maxValue - values[i, 0]), new Scalar(scalar.Val2, scalar.Val1, scalar.Val0, 255));
            }
            //Cv2.Resize(color, color, new OpenCvSharp.Size(HistogramSize.Width, HistogramSize.Height));
            return color;
        }

        private static (int maxValue, int[,] values) MatToArray(Mat image)
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

            return new(maxValue, pixelValues);
        }
    }
}
