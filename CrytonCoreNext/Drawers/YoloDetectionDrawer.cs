using CrytonCoreNext.AI.Models;
using CrytonCoreNext.Extensions;
using OpenCvSharp;
using OpenCvSharp.WpfExtensions;
using System.Collections.Generic;
using System.Windows.Media.Imaging;

namespace CrytonCoreNext.Drawers
{
    public static class YoloDetectionDrawer
    {
        private const int Thickness = 2;

        public static Mat DrawPredicitons(Mat mat, List<YoloPrediction> predicitons)
        {
            foreach (var prediction in predicitons)
            {
                if (prediction == null) continue;
                var color = new Scalar(
                    prediction.Label.Color.R,
                    prediction.Label.Color.G,
                    prediction.Label.Color.B);
                var rect = new Rect(
                    (int)prediction.Rectangle.X,
                    (int)prediction.Rectangle.Y,
                    (int)prediction.Rectangle.Width,
                    (int)prediction.Rectangle.Height);
                Cv2.Rectangle(mat, rect, color, Thickness);
            }
            return mat;
        }

        public static WriteableBitmap DrawDetection(AIImage selectedImage, AIDetectionImage detectionImage)
        {
            if (detectionImage == null)
            {
                return selectedImage.Image;
            }
            using var mat = selectedImage.Image.ToMat();
            using var overlay = new Mat(mat.Size(), mat.Type(), new Scalar(0, 0, 0));
            using var roi = new Mat(mat, detectionImage.Prediction.Rectangle.ToRect());
            using var combined = new Mat();
            Cv2.AddWeighted(mat, 0.5, overlay, 0.5, 0, combined);
            using var dest = new Mat(combined, detectionImage.Prediction.Rectangle.ToRect());
            roi.CopyTo(dest); 
            return combined.ToWriteableBitmap();
        }

        public static WriteableBitmap DrawAllDetections(AIImage selectedImage)
        {
            using var mat = selectedImage.Image.ToMat();
            foreach (var detection in selectedImage.Predictions)
            {
                Cv2.Rectangle(mat, detection.Rectangle.ToRect(), detection.Label.Color.ToScalar(), Thickness);
            }
            return mat.ToWriteableBitmap();
        }
    }
}
