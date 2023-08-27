using CrytonCoreNext.AI.Interfaces;
using CrytonCoreNext.AI.Models;
using CrytonCoreNext.AI.Utils;
using CrytonCoreNext.Drawers;
using OpenCvSharp;
using OpenCvSharp.Extensions;
using OpenCvSharp.WpfExtensions; 
using System.Collections.Generic;
using System.Windows.Media.Imaging;

namespace CrytonCoreNext.AI
{
    public class YoloModelService : IYoloModelService
    {
        private YoloV7 _yolov7;

        public bool LoadYoloModel(string path, bool useCUDA = false)
        {
            _yolov7 = new YoloV7(path, useCUDA);
            return _yolov7 != null;
        }

        public void LoadLabels(string pathToLabelsFile = "")
        {
            if (pathToLabelsFile == "")
            {
                _yolov7.SetupYoloDefaultLabels();
            }
        }

        public WriteableBitmap PredictAndDraw(Mat mat)
        {
            using var image = mat.ToBitmap(System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            var predicitons = _yolov7.Predict(image);
            return YoloDetectionDrawer.DrawPredicitons(mat, predicitons).ToWriteableBitmap();
        }

        public List<YoloPrediction> GetPredictions(Mat mat)
        {
            using var image = mat.ToBitmap(System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            return _yolov7.Predict(image);
        }
    }
}
