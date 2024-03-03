﻿using CrytonCoreNext.AI.Models;
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

        public static WriteableBitmap DrawDetection(AIImage selectedImage, AIDetectionImage? detectionImage)
        {
            if (detectionImage == null)
            {
                return selectedImage.AdjusterImage;
            }
            using var mat = selectedImage.AdjusterImage.ToMat();
            using var overlay = new Mat(mat.Size(), mat.Type(), new Scalar(0, 0, 0));
            var rectangle = CastPrediction(detectionImage.Prediction.Rectangle.ToRect(), mat.Size(), new Size(selectedImage.Image.Width, selectedImage.Image.Height));
            using var roi = new Mat(mat, rectangle);
            using var combined = new Mat();
            Cv2.AddWeighted(mat, 0.5, overlay, 0.5, 0, combined);
            using var dest = new Mat(combined, rectangle);
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

        private static Rect CastPrediction(Rect rect, Size sizeNew, Size sizeOld)
        {
            var wRatio = (double)sizeNew.Width / (double)sizeOld.Width;
            var hRatio = (double)sizeNew.Height / (double)sizeOld.Height;
            return new Rect((int)(rect.X * wRatio), (int)(rect.Y * hRatio), (int)(rect.Width * wRatio), (int)(rect.Height * hRatio));
        }
    }
}
