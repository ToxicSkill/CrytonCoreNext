using OpenCvSharp;

namespace CrytonCoreNext.Drawers
{
    public static class ImageDrawer
    {
        public static Mat ApplyAll(Mat mat, double strenght, double contrast, double brightness, double exposure, bool normalizeHistogram)
        {
            using var labColorMat = new Mat();
            Cv2.CvtColor(mat, labColorMat, ColorConversionCodes.BGR2Lab);
            var channels = Cv2.Split(labColorMat);
            Cv2.CreateCLAHE(2.0, new(8, 8)) .Apply(channels[0], channels[0]);
            Cv2.Merge(channels, labColorMat);
            Cv2.CvtColor(labColorMat, labColorMat, ColorConversionCodes.Lab2LBGR);
            Cv2.AddWeighted(mat, 1 - strenght, labColorMat,  strenght, 0, labColorMat);
            using var brightnessMat = SetBrightness(labColorMat, contrast, (int)brightness);
            using var exposureMat = SetExposure(brightnessMat, exposure);
            if (normalizeHistogram)
            {
                Cv2.Normalize(exposureMat, exposureMat); 
            }
            return exposureMat.Clone(); 
        }

        private static Mat SetBrightness(Mat mat, double contrast, int brightness)
        {
            var bcMat = new Mat();
            Cv2.ConvertScaleAbs(mat, bcMat, contrast, brightness);
            return bcMat;
        }

        private static Mat SetExposure(Mat mat, double exposure)
        {
            var exMat = new Mat();
            Cv2.AddWeighted(mat, 1,  mat, 1 + exposure, 1, exMat);
            return exMat;
        }
    }
}
