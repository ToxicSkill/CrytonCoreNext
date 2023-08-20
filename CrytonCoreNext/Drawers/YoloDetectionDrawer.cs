using CrytonCoreNext.AI.Models;
using OpenCvSharp;
using System.Collections.Generic;

namespace CrytonCoreNext.Drawers
{
    public static class YoloDetectionDrawer
    {
        private const int Thickness = 3;

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
    }
}
