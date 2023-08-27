using OpenCvSharp;

namespace CrytonCoreNext.Drawers
{
    public static class ImageDrawer
    {
        public static Mat DrawAutoColor(Mat mat, double strenght)
        {
            using var labColorMat = new Mat();
            Cv2.CvtColor(mat, labColorMat, ColorConversionCodes.BGR2Lab);
            var channels = Cv2.Split(labColorMat);
            Cv2.CreateCLAHE(5.0, new(3, 3)) .Apply(channels[0], channels[0]);
            Cv2.Merge(channels, labColorMat);
            Cv2.CvtColor(labColorMat, labColorMat, ColorConversionCodes.Lab2LBGR);
            Cv2.AddWeighted(mat, 1 - strenght, labColorMat,  strenght, 0, labColorMat);
            return labColorMat.Clone();
        }
    }
}
