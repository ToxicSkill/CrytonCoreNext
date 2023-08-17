using OpenCvSharp;

namespace CrytonCoreNext.Drawers
{
    public static class ColorGradientGenerator
    {
        public static Mat GenerateGradientImage(Size size, Scalar startColor, Scalar endColor) => 
            GenerateGradient(size, startColor, endColor);

        public static Mat GenerateGradient(Size size, Scalar startColor, Scalar endColor)
        {
            var gradientImage = new Mat(size, MatType.CV_8UC3, startColor);

            for (var x = 0; x < size.Width; x++)
            {
                var alpha = (double)x / (size.Width - 1);

                var currentColor = new Scalar
                (
                    (1 - alpha) * startColor.Val0 + alpha * endColor.Val0,
                    (1 - alpha) * startColor.Val1 + alpha * endColor.Val1,
                    (1 - alpha) * startColor.Val2 + alpha * endColor.Val2
                );

                using var column = new Mat(size.Height, 1, MatType.CV_8UC3, currentColor);
                var dest = gradientImage.ColRange(x, x + 1);
                column.CopyTo(dest);
            }

            return gradientImage;
        }
    }
}
