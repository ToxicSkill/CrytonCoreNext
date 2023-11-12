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
            Cv2.ConvertScaleAbs(labColorMat, labColorMat, alpha, beta);
            return labColorMat.Clone();
        }
    }
}
