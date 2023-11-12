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
        private static readonly Size HistogramSize = new (300, 80);

        public static WriteableBitmap CalcualteHistogram(WriteableBitmap image)
        {
            Cv2.Split(image.ToMat(), out Mat[] planes);

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
            using var histogramMat = new Mat(new OpenCvSharp.Size(HistogramSize.Width, HistogramSize.Height), MatType.CV_8UC4, new Scalar(0, 0, 0, 0));
            var rMat = DrawColorOnHistogram(r.values, Scalar.Red, maxValue);
            var gMat = DrawColorOnHistogram(g.values, Scalar.Green, maxValue);
            var bMat = DrawColorOnHistogram(b.values, Scalar.Blue, maxValue);
            Cv2.Add(histogramMat, bMat, histogramMat);
            Cv2.Add(histogramMat, gMat, histogramMat);
            Cv2.Add(histogramMat, rMat, histogramMat);
            Cv2.GaussianBlur(histogramMat, histogramMat, new OpenCvSharp.Size(3, 3), 0);
            return histogramMat.ToWriteableBitmap();
        }

        private static Mat DrawColorOnHistogram(int[,] values, Scalar scalar, int maxValue)
        {
            var color = new Mat(new OpenCvSharp.Size(256, maxValue), MatType.CV_8UC4, new Scalar(0, 0, 0, 0));
            for (var i = 0; i < 255; i++)
            {
                Cv2.Line(color, new OpenCvSharp.Point(i, maxValue), new OpenCvSharp.Point(i, maxValue - values[i, 0]), new Scalar(scalar.Val2, scalar.Val1, scalar.Val0, 255));
            }
            Cv2.Resize(color, color, new OpenCvSharp.Size(HistogramSize.Width, HistogramSize.Height));
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
