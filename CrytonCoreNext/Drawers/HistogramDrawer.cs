using OpenCvSharp;
using OpenCvSharp.WpfExtensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media.Imaging;

namespace CrytonCoreNext.Drawers
{
    public static class HistogramDrawer
    {
        private const double ResizeRatio = 0.2;

        private const int MaxMatDimensionSize = 256;

        private static readonly Scalar OpacityScalar = new(0, 0, 0, 0);

        private static readonly Size HistogramSize = new(MaxMatDimensionSize, 80);

        private static readonly int[] _histSize = [MaxMatDimensionSize];

        private static readonly Rangef[] _ranges = [new(0, MaxMatDimensionSize)];

        public static WriteableBitmap CalcualteHistogram(Mat image)
        {
            using var matForHistogram = new Mat();
            var maxDimension = new List<int>() { image.Width, image.Height }.Max();
            var newSize = new Size(image.Width * ResizeRatio, image.Height * ResizeRatio);
            if (newSize.Width > MaxMatDimensionSize || newSize.Height > MaxMatDimensionSize)
            {
                var doubles = new List<double>() { ((double)MaxMatDimensionSize / (double)newSize.Width), ((double)MaxMatDimensionSize / (double)newSize.Height) };
                var newRatio = doubles.Min();
                newSize = new Size(newSize.Width * newRatio, newSize.Height * newRatio);
            }
            Cv2.Resize(image, matForHistogram, newSize);
            Cv2.Split(matForHistogram, out Mat[] planes);
            var results = new List<(int max, int[,] values)>();
            for (var i = 0; i < planes.Length; i++)
            {
                Cv2.CalcHist([planes[i]], [0], null, planes[i], 1, _histSize, _ranges);
                Cv2.Normalize(planes[i], planes[i], 0, MaxMatDimensionSize, NormTypes.MinMax);
                results.Add(MatToArray(planes[i]));
            }

            var maxValue = results.Max(x => x.max);
            maxValue = Math.Clamp(maxValue, 1, maxValue);
            using var histogramMat = new Mat(new Size(MaxMatDimensionSize, maxValue), MatType.CV_8UC4, OpacityScalar);
            for (var i = 0; i < results.Count; i++)
            {
                var color = new Scalar(i == 2 ? 255 : 0, i == 1 ? 255 : 0, i == 0 ? 255 : 0, 255);
                Cv2.Add(histogramMat, DrawColorOnHistogram(results[i].values, color, results[i].max, maxValue), histogramMat);
            }
            Cv2.Resize(histogramMat, histogramMat, HistogramSize);
            return histogramMat.ToWriteableBitmap();
            //var polynomials = new List<Polynomial>();
            //var range = Enumerable.Range(0, MaxMatDimensionSize).Select(c => (double)c).ToArray();
            //int[] oDArray = new int[MaxMatDimensionSize];
            //foreach (var color in new List<int[,]>() { r.values, g.values, b.values })
            //{
            //    System.Buffer.BlockCopy(color, 0, oDArray, 0, MaxMatDimensionSize * 4);
            //    double[] result = oDArray.Select(x => (double)x).ToArray();
            //    polynomials.Add(Polynomial.Fit(result, range, 15));
            //}
            //var evaluations = new List<List<Point>>();
            //var maxes = new List<int>();
            //foreach (var polynomial in polynomials)
            //{
            //    var points = new List<Point>();
            //    for (var i = 0; i < MaxMatDimensionSize; i++)
            //    {
            //        points.Add(new Point(i, (int)polynomial.Evaluate(i)));
            //    }
            //    evaluations.Add(points);
            //    maxes.Add(points.Max(x => x.Y));
            //}
            //var max = maxes.Max();
            //for (var i = 0; i < evaluations.Count; i++)
            //{
            //    var color = new Scalar(i == 2 ? 255 : 0, i == 1 ? 255 : 0, i == 0 ? 255 : 0, 255);
            //    for (var j = 0; j < evaluations[i].Count; j++)
            //    {
            //        Cv2.Circle(histogramMat, evaluations[i][j], 5, color);
            //    }
            //}
        }

        private static Mat DrawColorOnHistogram(int[,] values, Scalar scalar, int selfMax, int maxValue)
        {
            if (selfMax == 0)
            {
                selfMax = 1;
            }
            if (maxValue == 0)
            {
                maxValue = 1;
            }
            var color = new Mat(new Size(MaxMatDimensionSize, selfMax), MatType.CV_8UC4, new Scalar(0, 0, 0, 0));
            for (var i = 0; i < MaxMatDimensionSize; i++)
            {
                Cv2.Line(color, new Point(i, selfMax), new Point(i, selfMax - values[i, 0]), new Scalar(scalar.Val2, scalar.Val1, scalar.Val0, 255));
            }
            Cv2.Resize(color, color, new Size(MaxMatDimensionSize, maxValue));
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
