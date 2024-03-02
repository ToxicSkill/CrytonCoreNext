using OpenCvSharp;
using OpenCvSharp.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Shapes;

namespace CrytonCoreNext.Drawers
{
    public static class HistogramDrawer
    {
        private const double ResizeRatio = 0.2;

        private const int MaxMatDimensionSize = 256;

        private const int InterpolationFactor = 2;

        private static readonly Scalar OpacityScalar = new(0, 0, 0, 0);

        private static readonly Size HistogramSize = new(MaxMatDimensionSize * InterpolationFactor, 80 * InterpolationFactor);

        private static readonly int[] _histSize = [MaxMatDimensionSize];

        private static readonly Rangef[] _ranges = [new(0, MaxMatDimensionSize)];

        public static System.Drawing.Bitmap CalcualteHistogram(Mat image)
        {
            using var matForHistogram = new Mat();
            var maxDimension = new List<int>() { image.Width, image.Height }.Max();
            var newSize = new Size(image.Width * ResizeRatio, image.Height * ResizeRatio);
            if (newSize.Width > MaxMatDimensionSize || newSize.Height > MaxMatDimensionSize)
            {
                var doubles = new List<double>() { MaxMatDimensionSize / (double)newSize.Width, MaxMatDimensionSize / (double)newSize.Height };
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
            using var histogramMat = new Mat(new Size(MaxMatDimensionSize * InterpolationFactor, maxValue), MatType.CV_8UC4, OpacityScalar);
            for (var i = 0; i < results.Count; i++)
            {
                var color = new Scalar(i == 2 ? 200 : 0, i == 1 ? 200 : 0, i == 0 ? 200 : 0, 255);
                var interpolated = MatToArrayWithInterpolation(planes[i]);
                Cv2.Add(histogramMat, DrawColorOnHistogram(interpolated.values, color, interpolated.maxValue, maxValue), histogramMat);
            }
            Cv2.Resize(histogramMat, histogramMat, HistogramSize);
            foreach (var plane in planes)
            {
                plane.Dispose();
            }

            return histogramMat.ToBitmap();
        }

        public static List<Path> CalcualteHistogram2(Mat image)
        {
            using var matForHistogram = new Mat();
            var maxDimension = new List<int>() { image.Width, image.Height }.Max();
            var newSize = new Size(image.Width * ResizeRatio, image.Height * ResizeRatio);
            if (newSize.Width > MaxMatDimensionSize || newSize.Height > MaxMatDimensionSize)
            {
                var doubles = new List<double>() { MaxMatDimensionSize / (double)newSize.Width, MaxMatDimensionSize / (double)newSize.Height };
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
            var paths = new List<Path>();
            for (var i = 0; i < results.Count; i++)
            {
                var color = new Scalar(i == 2 ? 200 : 0, i == 1 ? 200 : 0, i == 0 ? 200 : 0, 255);
                paths.Add(BezierDrawer.DrawableBezier(PrepareResultsForBezier(results[i].values), color));
                //Cv2.Add(histogramMat, DrawColorOnHistogram(results[i].values, color, results[i].max, maxValue), histogramMat);
            }
            return paths;
        }


        private static System.Windows.Point[] PrepareResultsForBezier(int[,] values)
        {
            var points = new System.Windows.Point[values.Length];
            for (var i = 0; i < values.Length; i++)
            {
                points[i] = new System.Windows.Point(i, values[i, 0]);
            }
            return points;
        }

        private static IEnumerable<IEnumerable<Point>> PrepareResults(int[,] values)
        {
            var points = new List<Point>();
            for (int i = 0; i < values.Length; i++)
            {
                points.Add(new(i, values[i, 0]));
            }
            return [InterpolateList(points)];
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
            var color = new Scalar(scalar.Val2, scalar.Val1, scalar.Val0, 255);
            var colorMat = new Mat(new Size(MaxMatDimensionSize * InterpolationFactor, selfMax), MatType.CV_8UC4, new Scalar(0, 0, 0, 0));
            for (var i = 0; i < MaxMatDimensionSize * InterpolationFactor; i++)
            {
                Cv2.Line(colorMat, new Point(i, selfMax), new Point(i, values[i, 0]), color);
            }
            Cv2.Resize(colorMat, colorMat, new Size(MaxMatDimensionSize * InterpolationFactor, maxValue));
            return colorMat;
        }
        private static List<Point> InterpolateList(List<Point> originalList)
        {
            List<Point> interpolatedList = [];
            var index = 0;
            for (var i = 0; i < originalList.Count - 1; i++)
            {
                double startValue = originalList[i].Y;
                double endValue = originalList[i + 1].Y;

                for (var j = 0; j < InterpolationFactor; j++)
                {
                    double interpolatedValue = Interpolate(startValue, endValue, (double)j / InterpolationFactor);
                    interpolatedList.Add(new(index, InterpolationFactor * (MaxMatDimensionSize - interpolatedValue)));
                    index++;
                }
            }
            interpolatedList.Add(new(index, InterpolationFactor * (MaxMatDimensionSize - originalList[originalList.Count - 1].Y)));

            return interpolatedList;
        }

        private static double Interpolate(double start, double end, double ratio)
        {
            return start + (end - start) * ratio;
        }
        private static (int maxValue, int[,] values) MatToArrayWithInterpolation(Mat image)
        {
            var pixelValues = new int[image.Rows * InterpolationFactor, image.Cols];
            for (var col = 0; col < image.Cols; col++)
            {
                var index = 0;
                for (var row = 0; row < image.Rows; row++)
                {
                    double startValue = image.At<float>(row, col);
                    double endValue = image.At<float>(row + 1, col);

                    for (var j = 0; j < InterpolationFactor; j++)
                    {
                        double interpolatedValue = Interpolate(startValue, endValue, (double)j / InterpolationFactor);
                        pixelValues[index, col] = (int)(InterpolationFactor * (MaxMatDimensionSize - interpolatedValue));
                        index++;
                    }
                }
                pixelValues[index - 1, col] = (int)(InterpolationFactor * (MaxMatDimensionSize - image.At<float>(image.Rows - 1, col)));
            }

            return new(pixelValues.Cast<int>().Max(), pixelValues);
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
