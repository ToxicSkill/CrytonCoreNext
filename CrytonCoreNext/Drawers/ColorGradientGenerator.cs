using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrytonCoreNext.Drawers
{
    public static class ColorGradientGenerator
    {
        public static Mat GenerateGradientImage(Size size, Scalar startColor, Scalar endColor, bool vertical = false)
        {
            using var retVal = GenerateGradientImage(size.Width, size.Height, startColor, endColor);
            Cv2.ImWrite("C:\\Users\\gizmo\\OneDrive\\Obrazy\\ret.png", retVal);
            return retVal;
        }

        static Mat GenerateGradientImage(int width, int height, Scalar startColor, Scalar endColor)
        {
            Mat gradientImage = new Mat(height, width, MatType.CV_8UC3);

            for (int y = 0; y < height; y++)
            {
                double t = (double)y / (height - 1);
                Scalar interpolatedColor = InterpolateColor(startColor, endColor, y);

                for (int x = 0; x < width; x++)
                {
                    gradientImage.Set(y, x, interpolatedColor);
                }
            }

            return gradientImage;
        }

        static Scalar InterpolateColor(Scalar startColor, Scalar endColor, double t)
        {
            double r = t;
            double g = 0;
            double b = 0;

            return new Scalar(b, g, r); // OpenCV używa kolejności BGR
        }
    }
}
