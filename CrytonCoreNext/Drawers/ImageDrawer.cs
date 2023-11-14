using Microsoft.Extensions.Logging;
using OpenCvSharp;

namespace CrytonCoreNext.Drawers
{
    public static class ImageDrawer
    {
        public static Mat ApplyAll(Mat mat, double strenght, double alpha, double beta)
        {
            using var labColorMat = new Mat();
            Cv2.CvtColor(mat, labColorMat, ColorConversionCodes.BGR2Lab);
            var channels = Cv2.Split(labColorMat);
            Cv2.CreateCLAHE(2.0, new(8, 8)) .Apply(channels[0], channels[0]);
            Cv2.Merge(channels, labColorMat);
            Cv2.CvtColor(labColorMat, labColorMat, ColorConversionCodes.Lab2LBGR);
            Cv2.AddWeighted(mat, 1 - strenght, labColorMat,  strenght, 0, labColorMat);
            using var brightnessMat = SetBrightness(labColorMat, (int)beta);
            return brightnessMat.Clone();
        }

        private static Mat SetBrightness(Mat mat, int brightness)
        {
            using var hsv = new Mat();
            Cv2.CvtColor(mat, hsv, ColorConversionCodes.BGR2HSV);
            var hsvChannles = Cv2.Split(hsv);
            var lim = 255 - brightness;

            for (var i = 0; i < hsv.Height; i++)
            {
                for (var j = 0; j < hsv.Width; j++)
                {
                    var value = hsvChannles[2].Get<byte>(i, j);
                    if (value > lim)
                    {
                        hsvChannles[2].Set<byte>(i, j, 255);
                    }
                    else
                    {
                        hsvChannles[2].Set<byte>(i, j, (byte)(value+brightness));
                    }
                }
            }

            Cv2.Merge(hsvChannles, hsv); 
            Cv2.CvtColor(hsv, mat, ColorConversionCodes.HSV2BGR);
            return mat;
        }
    }
}
