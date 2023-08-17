using OpenCvSharp;
using OpenCvSharp.WpfExtensions;
using System.Windows.Media.Imaging;

namespace CrytonCoreNext.Drawers
{
    public static class ColorGradientGenerator
    {
        public static WriteableBitmap GenerateGradient(Size size, Scalar startColor, Scalar endColor)
        {
            var gradientImage = new Mat(size, MatType.CV_8UC4, startColor);

            for (var x = 0; x < size.Width; x++)
            {
                var alpha = (double)x / (size.Width - 1);

                var currentColor = new Scalar
                (
                    (1 - alpha) * startColor.Val0 + alpha * endColor.Val0,
                    (1 - alpha) * startColor.Val1 + alpha * endColor.Val1,
                    (1 - alpha) * startColor.Val2 + alpha * endColor.Val2,
                    (1 - alpha) * startColor.Val3 + alpha * endColor.Val3
                );

                using var column = new Mat(size.Height, 1, MatType.CV_8UC4, currentColor);
                var dest = gradientImage.ColRange(x, x + 1);
                column.CopyTo(dest);
            }

            return gradientImage.ToWriteableBitmap();
        }
    }
}
